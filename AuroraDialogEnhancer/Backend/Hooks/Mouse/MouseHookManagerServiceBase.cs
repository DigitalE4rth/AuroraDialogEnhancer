using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using NativeMethods = AuroraDialogEnhancer.Backend.External.NativeMethods;

namespace AuroraDialogEnhancer.Backend.Hooks.Mouse;

public abstract class MouseHookManagerServiceBase
{
    protected const int WH_MOUSE_LL = 14;

    protected readonly IntPtr _mouseMove        = new(0x0200);
    protected readonly IntPtr _leftButtonDown   = new(0x0201);
    protected readonly IntPtr _leftButtonUp     = new(0x0202);
    protected readonly IntPtr _rightButtonDown  = new(0x0204);
    protected readonly IntPtr _rightButtonUp    = new(0x0205);
    protected readonly IntPtr _middleButtonDown = new(0x0207);
    protected readonly IntPtr _middleButtonUp   = new(0x0208);
    protected readonly IntPtr _mouseWheel       = new(0x020A);
    protected readonly IntPtr _xButtonDown      = new(0x020B);
    protected readonly IntPtr _xButtonUp        = new(0x02);
    protected readonly IntPtr _primaryDown;
    protected readonly IntPtr _primaryUp;

    protected Action<bool, Point>? _primaryClickAction;

    protected NativeMethods.HookProc? _hook;
    protected IntPtr _hookId;
    protected bool _isStarted;

    protected Dictionary<IntPtr, Action<IntPtr, IntPtr>> _registeredLowKeyBinds;

    protected Action? _middleButtonAction;
    protected Action? _wheelUpAction;
    protected Action? _wheelDownAction;
    protected Action? _backButtonAction;
    protected Action? _forwardButtonAction;

    protected MouseHookManagerServiceBase()
    {
        _primaryDown = NativeMethods.IsMousePrimaryButtonSwapped ? _rightButtonDown : _leftButtonDown;
        _primaryUp = NativeMethods.IsMousePrimaryButtonSwapped ? _rightButtonUp : _leftButtonUp;
        _hookId = IntPtr.Zero;
        _registeredLowKeyBinds = new Dictionary<IntPtr, Action<IntPtr, IntPtr>>();
    }

    public void Start()
    {
        if (_isStarted) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _hook = HookCallback;
            _hookId = NativeMethods.SetWindowsHookEx(WH_MOUSE_LL, _hook, NativeMethods.User32Pointer, 0);
            _isStarted = true;
        });
    }

    public virtual void Stop()
    {
        if (!_isStarted) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWindowsHookEx(_hookId);
            _isStarted = false;
        });
    }

    public void RegisterHotKey(int mouseKey, Action action) => RegisterHotKey((EHighMouseKey)mouseKey, action);

    public void RegisterHotKey(EHighMouseKey mouseKey, Action action)
    {
        if (mouseKey == EHighMouseKey.MiddleButton)
        {
            _registeredLowKeyBinds.Add(_middleButtonDown, HandleMiddleDown);
            _middleButtonAction = action;
            return;
        }

        if (mouseKey == EHighMouseKey.MouseWheelUp)
        {
            if (!_registeredLowKeyBinds.ContainsKey(_mouseWheel))
            {
                _registeredLowKeyBinds.Add(_mouseWheel, HandleWheelAction);
            }

            _wheelUpAction = action;
            return;
        }

        if (mouseKey == EHighMouseKey.MouseWheelDown)
        {
            if (!_registeredLowKeyBinds.ContainsKey(_mouseWheel))
            {
                _registeredLowKeyBinds.Add(_mouseWheel, HandleWheelAction);
            }

            _wheelDownAction = action;
            return;
        }

        if (mouseKey == EHighMouseKey.Back)
        {
            if (!_registeredLowKeyBinds.ContainsKey(_xButtonDown))
            {
                _registeredLowKeyBinds.Add(_xButtonDown, HandleXButton);
            }

            _backButtonAction = action;
            return;
        }

        if (mouseKey == EHighMouseKey.Forward)
        {
            if (!_registeredLowKeyBinds.ContainsKey(_xButtonDown))
            {
                _registeredLowKeyBinds.Add(_xButtonDown, HandleXButton);
            }

            _forwardButtonAction = action;
        }
    }

    public void RegisterPrimaryClick(Action<bool, Point> action)
    {
        _registeredLowKeyBinds.Add(_primaryDown, HandlePrimaryDown);
        _registeredLowKeyBinds.Add(_primaryUp,   HandlePrimaryUp);
        _primaryClickAction = action;
    }

    public void UnRegisterPrimaryClick()
    {
        _registeredLowKeyBinds.Remove(_primaryDown);
        _registeredLowKeyBinds.Remove(_primaryUp);
        _primaryClickAction = null;
    }

    public void UnRegisterHotKey(int mouseKey) => UnRegisterHotKey((EHighMouseKey)mouseKey);

    public void UnRegisterHotKey(EHighMouseKey mouseKey)
    {
        if (mouseKey == EHighMouseKey.MiddleButton)
        {
            _registeredLowKeyBinds.Remove(_middleButtonDown);
            _middleButtonAction = null;
            return;
        }

        if (mouseKey == EHighMouseKey.MouseWheelUp)
        {
            _registeredLowKeyBinds.Remove(_mouseWheel);
            _wheelUpAction = null;
            return;
        }

        if (mouseKey == EHighMouseKey.MouseWheelDown)
        {
            _registeredLowKeyBinds.Remove(_mouseWheel);
            _wheelDownAction = null;
            return;
        }

        if (mouseKey == EHighMouseKey.Back)
        {
            _registeredLowKeyBinds.Remove(_xButtonDown);
            _backButtonAction = null;
            return;
        }

        if (mouseKey == EHighMouseKey.Forward)
        {
            _registeredLowKeyBinds.Remove(_xButtonDown);
            _forwardButtonAction = null;
        }
    }

    public void UnRegisterAll()
    {
        _registeredLowKeyBinds.Clear();
        _wheelDownAction = null;
        _wheelUpAction = null;
        _backButtonAction = null;
        _forwardButtonAction = null;
        _middleButtonAction = null;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        var nextHookPtr = NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);

        if (wParam == _mouseMove || !_registeredLowKeyBinds.ContainsKey(wParam) || nCode < 0)
        {
            return nextHookPtr;
        }

        Task.Run(() => { _registeredLowKeyBinds[wParam].Invoke(wParam, lParam); }).ConfigureAwait(false);

        return nextHookPtr;
    }

    private void HandlePrimaryDown(IntPtr wParam, IntPtr lParam)
    {
        _primaryClickAction!.Invoke(true, new Point(Cursor.Position.X, Cursor.Position.Y));
    }

    private void HandlePrimaryUp(IntPtr wParam, IntPtr lParam)
    {
        _primaryClickAction!.Invoke(false, new Point(Cursor.Position.X, Cursor.Position.Y));
    }

    private void HandleWheelAction(IntPtr wParam, IntPtr lParam)
    {
        var mouseData = (LowLevelMouseHookStruct)Marshal.PtrToStructure(lParam, typeof(LowLevelMouseHookStruct));
        var delta = mouseData.mouseData >> 16;

        switch (delta)
        {
            case -120 when _wheelDownAction is not null:
                _wheelDownAction.Invoke();
                return;
            case 120 when _wheelUpAction is not null:
                _wheelUpAction.Invoke();
                return;
            default:
                return;
        }
    }

    private void HandleXButton(IntPtr wParam, IntPtr lParam)
    {
        var mouseData = (LowLevelMouseHookStruct)Marshal.PtrToStructure(lParam, typeof(LowLevelMouseHookStruct));
        var numButton = mouseData.mouseData >> 16;
        switch (numButton)
        {
            case 1 when _backButtonAction is not null:
                _backButtonAction.Invoke();
                return;
            case 2 when _forwardButtonAction is not null:
                _forwardButtonAction.Invoke();
                return;
            default:
                return;
        }
    }

    private void HandleMiddleDown(IntPtr wParam, IntPtr lParam)
    {
        _middleButtonAction?.Invoke();
    }
}
