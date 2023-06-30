using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AuroraDialogEnhancer.Backend.External;

namespace AuroraDialogEnhancer.Backend.Hooks.Keyboard;

/// <summary>
/// A hot key manager that uses a low-level global keyboard hook, but eventually only fires events for
/// pre-registered hot keys, i.e. not invading a user's privacy.
/// </summary>
/// <remarks>
/// Modified version of <b>kfirprods's</b> <see href="https://github.com/kfirprods/NonInvasiveKeyboardHook">NonInvasiveKeyboardHook</see>
/// </remarks>
public abstract class KeyboardHookManagerServiceBase
{
    protected readonly ModifierKeysProvider _modifierKeysProvider;

    // Source: https://blogs.msdn.microsoft.com/toub/2006/05/03/low-level-keyboard-hook-in-c/
    protected const int WH_KEYBOARD_LL = 13;
    protected const int WM_KEYDOWN     = 0x0100;
    protected const int WM_KEYUP       = 0x0101;
    protected const int WM_SYSKEYDOWN  = 0x0104;
    protected const int WM_SYSKEYUP    = 0x0105;

    /// <summary>
    /// Keeps track of all registered hot keys
    /// </summary>
    private readonly Dictionary<KeyBindStruct, Action> _registeredKeyBinds;

    /// <summary>
    /// Keeps track of all keys that are held down to prevent firing callbacks
    /// more than once for a single keypress
    /// </summary>
    protected HashSet<int> DownKeys { get; }

    protected SortedSet<int> DownModifierKeys { get; }

    private NativeMethods.HookProc? _hook;
    private IntPtr _hookId = IntPtr.Zero;
    private bool   _isStarted;
    protected readonly object _modifiersLock;

    /// <summary>
    /// Instantiates an empty keyboard hook manager.
    /// It is best practice to keep a single instance per process.
    /// <see cref="Start"/> must be called to start the low-level keyboard hook manager
    /// </summary>
    protected KeyboardHookManagerServiceBase(ModifierKeysProvider modifierKeysProvider)
    {
        _modifierKeysProvider = modifierKeysProvider;
        _registeredKeyBinds   = new Dictionary<KeyBindStruct, Action>();
        DownKeys         = new HashSet<int>();
        DownModifierKeys = new SortedSet<int>();
        _modifiersLock   = new object();
    }

    /// <summary>
    /// Starts the low-level keyboard hook.
    /// Hot keys can be registered regardless of the low-level keyboard hook's state, but their callbacks will only ever be invoked if the low-level keyboard hook is running and intercepting keys.
    /// </summary>
    public void Start()
    {
        if (_isStarted) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _hook   = HookCallback;
            _hookId = SetHook(_hook);
        });

        _isStarted = true;
    }

    /// <summary>
    /// Pauses the low-level keyboard hook (without un-registering the existing hot keys).
    /// </summary>
    public void Stop()
    {
        if (!_isStarted) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWindowsHookEx(_hookId);
        });

        DownKeys.Clear();
        DownModifierKeys.Clear();

        _isStarted = false;
    }

    public void RegisterHotKey(int regularKey, Action action)
    {
        RegisterHotKeys(Enumerable.Empty<int>(), regularKey, action);
    }

    public void RegisterHotKeys(IEnumerable<int> virtualKeyCodes, Action action)
    {
        var regularKeys = new List<int>();
        var modifierKeys = new List<int>();

        foreach (var virtualKeyCode in virtualKeyCodes)
        {
            if (_modifierKeysProvider.IsModifierKey(virtualKeyCode))
            {
                modifierKeys.Add(virtualKeyCode);
                continue;
            }

            regularKeys.Add(virtualKeyCode);
        }

        var keyBindStruct = regularKeys.Any()
            ? new KeyBindStruct(modifierKeys, regularKeys.First())
            : new KeyBindStruct(modifierKeys);

        if (_registeredKeyBinds.ContainsKey(keyBindStruct))
        {
            throw new HotkeyAlreadyRegisteredException();
        }

        _registeredKeyBinds[keyBindStruct] = action;
    }

    /// <summary>
    /// Registers a hot key.
    /// </summary>
    /// <param name="modifierKeys">Key bind struct to register.</param>
    /// <param name="action">The callback action to invoke when this hot key is pressed.</param>
    /// <exception cref="HotkeyAlreadyRegisteredException">Thrown when the given key is already mapped to a callback.</exception>
    public void RegisterHotKeys(IEnumerable<int> modifierKeys, int virtualKeyCode, Action action)
    {
        var keyBindStruct = new KeyBindStruct(modifierKeys, virtualKeyCode);

        if (_registeredKeyBinds.ContainsKey(keyBindStruct))
        {
            throw new HotkeyAlreadyRegisteredException();
        }

        _registeredKeyBinds[keyBindStruct] = action;
    }

    /// <summary>
    /// Un-registers a specific key by its unique identifier.
    /// </summary>
    /// <param name="keyBindStruct">Key bind struct to un-register.</param>
    public void UnRegisterHotKeys(KeyBindStruct keyBindStruct)
    {
        if (!_registeredKeyBinds.ContainsKey(keyBindStruct)) throw new HotkeyNotRegisteredException();

        _registeredKeyBinds.Remove(keyBindStruct);
    }

    public void UnRegisterHotKeys(IEnumerable<int> virtualKeyCodes)
    {
        var regularKeys  = new List<int>();
        var modifierKeys = new List<int>();

        foreach (var virtualKeyCode in virtualKeyCodes)
        {
            if (_modifierKeysProvider.IsModifierKey(virtualKeyCode))
            {
                modifierKeys.Add(virtualKeyCode);
                continue;
            }

            regularKeys.Add(virtualKeyCode);
        }

        var keyBindStruct = regularKeys.Any()
            ? new KeyBindStruct(modifierKeys, regularKeys.First())
            : new KeyBindStruct(modifierKeys);

        if (!_registeredKeyBinds.ContainsKey(keyBindStruct))
        {
            throw new HotkeyNotRegisteredException();
        }

        _registeredKeyBinds.Remove(keyBindStruct);
    }

    public void UnRegisterHotKeys(int virtualKeyCode)
    {
        UnRegisterHotKeys(new KeyBindStruct(Enumerable.Empty<int>(), virtualKeyCode));
    }

    /// <summary>
    /// Un-registers all hot keys (the low-level keyboard hook continues running).
    /// </summary>
    public void UnRegisterAll() => _registeredKeyBinds.Clear();

    #region Private methods
    protected void HandleKeyPress(int virtualKeyCode)
    {
        var keyBindStruct = new KeyBindStruct(DownModifierKeys, virtualKeyCode);
        if (!_registeredKeyBinds.TryGetValue(keyBindStruct, out var callbackAction)) return;

        Task.Run(callbackAction).ConfigureAwait(false);
    }
    #endregion

    #region Low level keyboard hook
    private IntPtr SetHook(NativeMethods.HookProc proc)
    {
        return NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, proc, NativeMethods.User32Pointer, 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        var nextHookPtr = NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);

        if (nCode < 0) return nextHookPtr;

        var virtualKeyCode = Marshal.ReadInt32(lParam);

        HandleSingleKeyboardInput(wParam, virtualKeyCode);

        return nextHookPtr;
    }

    protected virtual void HandleSingleKeyboardInput(IntPtr wParam, int virtualKeyCode)
    {
        var isModifier = _modifierKeysProvider.IsModifierKey(virtualKeyCode);

        // If the keyboard event is a KeyDown event (i.e. key pressed)
        if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
        {
            if (isModifier)
            {
                lock (_modifiersLock)
                {
                    DownModifierKeys.Add(virtualKeyCode);
                }
            }

            // Trigger callbacks that are registered for this key, but only once per key press
            if (!DownKeys.Contains(virtualKeyCode))
            {
                HandleKeyPress(virtualKeyCode);
                DownKeys.Add(virtualKeyCode);
                return;
            }
        }

        // If the keyboard event is a KeyUp event (i.e. key released)
        if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
        {
            if (isModifier)
            {
                lock (_modifiersLock)
                {
                    DownModifierKeys.Remove(virtualKeyCode);
                }
            }

            DownKeys.Remove(virtualKeyCode);
        }
    }
    #endregion
}
