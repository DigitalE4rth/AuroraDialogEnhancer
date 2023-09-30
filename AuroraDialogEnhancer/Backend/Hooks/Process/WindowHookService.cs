using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using AuroraDialogEnhancer.Backend.External;
using AuroraDialogEnhancer.Backend.Hooks.Game;

namespace AuroraDialogEnhancer.Backend.Hooks.Process;

public class WindowHookService
{
    private readonly ProcessInfoService     _processInfoService;
    private readonly HookedGameDataProvider _hookedGameDataProvider;

    #region Focus
    /// <summary>
    /// Pointer of the change focus event.
    /// </summary>
    private IntPtr _focusChangedHookPointer;

    /// <summary>
    /// Delegate of focus change hook.
    /// </summary>
    private readonly NativeMethods.WinEventDelegate _focusDelegate;

    public event EventHandler<bool>? OnFocusChanged;
    #endregion

    #region Minimization
    /// <summary>
    /// Pointer of the minimization end event.
    /// </summary>
    private IntPtr _minimizationHookPointer;

    /// <summary>
    /// Main Window minimization state
    /// </summary>
    private bool IsMinimized { get; set; }

    /// <summary>
    /// Semaphore for capturing <see cref="OnMinimizeEnd"/> event.
    /// </summary>
    private SemaphoreSlim? _minimizationEndSemaphore;

    /// <summary>
    /// Delegate of minimization end hook.
    /// </summary>
    private NativeMethods.WinEventDelegate _minimizationDelegate;

    public event EventHandler? OnMinimizeEnd;
    #endregion

    #region Location
    /// <summary>
    /// Target window pointer.
    /// </summary>
    private IntPtr _targetWindowPointer;

    /// <summary>
    /// Pointer of the location change event.
    /// </summary>
    private IntPtr _locationChangedHookPointer;

    /// <summary>
    /// Delegate of location change hook.
    /// </summary>
    private readonly NativeMethods.WinEventDelegate _locationDelegate;

    public event EventHandler? OnLocationChanged;
    #endregion

    public WindowHookService(ProcessInfoService     processInfoService,
                             HookedGameDataProvider hookedGameDataProvider)
    {
        _processInfoService     = processInfoService;
        _hookedGameDataProvider = hookedGameDataProvider;

        _focusDelegate        = FocusChangedHook;
        _minimizationDelegate = OnMinimizeEndHook;
        _locationDelegate     = LocationChangedHook;
    }

    #region Focus
    private bool IsTargetWindowForeground() => IsTargetWindowForeground(NativeMethods.GetForegroundWindow());

    private bool IsTargetWindowForeground(IntPtr focusedWindowHandle)
    {
        if (_hookedGameDataProvider.Data?.GameProcess is null) return false;

        NativeMethods.GetWindowThreadProcessId(focusedWindowHandle, out var activeProcId);

        return activeProcId == _hookedGameDataProvider.Data.GameProcess.Id;
    }

    public void InitializeFocusHook()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _focusChangedHookPointer = NativeMethods.SetWinEventHook(
                NativeMethods.EVENT_SYSTEM_FOREGROUND, NativeMethods.EVENT_SYSTEM_FOREGROUND, 
                IntPtr.Zero, 
                _focusDelegate, 
                0, 0, 
                NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS | NativeMethods.WINEVENT_SKIPOWNTHREAD);
        }, DispatcherPriority.Normal);

        OnFocusChanged?.Invoke(this, IsFocused());
    }

    private void FocusChangedHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        OnFocusChanged?.Invoke(this, IsFocused(hwnd));
    }

    public void SendFocusedEvent()
    {
        OnFocusChanged?.Invoke(this, IsFocused());
    }

    private void UnSetFocusHook()
    {
        if (_focusChangedHookPointer == IntPtr.Zero) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWinEvent(_focusChangedHookPointer);
            _focusChangedHookPointer = IntPtr.Zero;
        }, DispatcherPriority.Normal);
    }

    // Unity bug semi-fix
    // https://github.com/DigitalE4rth/AuroraDialogEnhancer/issues/9
    private bool IsFocused(IntPtr windowHandler)
    {
        if (IsMinimized && IsTargetWindowForeground(windowHandler)) return false;
        return !IsMinimized && IsTargetWindowForeground(windowHandler);
    }

    private bool IsFocused()
    {
        if (IsMinimized && IsTargetWindowForeground()) return false;
        return !IsMinimized && IsTargetWindowForeground();
    }
    #endregion

    #region Minimization
    public void InitializeMinimizationHook()
    {
        IsMinimized = _hookedGameDataProvider.Data!.GameWindowInfo!.GetMinimizationState();
        _minimizationDelegate = MinimizationChangedHook;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _minimizationHookPointer = NativeMethods.SetWinEventHook(
                NativeMethods.EVENT_SYSTEM_MINIMIZESTART, NativeMethods.EVENT_SYSTEM_MINIMIZEEND,
                IntPtr.Zero,
                _minimizationDelegate,
                (uint)_hookedGameDataProvider.Data!.GameProcess!.Id,
                0,
                NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS | NativeMethods.WINEVENT_SKIPOWNTHREAD);
        }, DispatcherPriority.Normal);
    }

    private void MinimizationChangedHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        switch (eventType)
        {
            case NativeMethods.EVENT_SYSTEM_MINIMIZESTART:
                IsMinimized = true;
                OnFocusChanged?.Invoke(this, IsFocused(hwnd));
                return;
            case NativeMethods.EVENT_SYSTEM_MINIMIZEEND:
                IsMinimized = false;
                OnFocusChanged?.Invoke(this, IsFocused(hwnd));
                return;
            default:
                IsMinimized = _hookedGameDataProvider.Data!.GameWindowInfo!.GetMinimizationState();
                OnFocusChanged?.Invoke(this, IsFocused(hwnd));
                break;
        }
    }

    private void SetMinimizeEndHook(uint processId)
    {
        _minimizationDelegate = OnMinimizeEndHook;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _minimizationHookPointer = NativeMethods.SetWinEventHook(
                NativeMethods.EVENT_SYSTEM_MINIMIZEEND, NativeMethods.EVENT_SYSTEM_MINIMIZEEND,
                IntPtr.Zero,
                _minimizationDelegate,
                processId, 0,
                NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS | NativeMethods.WINEVENT_SKIPOWNTHREAD);
        }, DispatcherPriority.Normal);
    }

    public void UnSetMinimizeEndHook()
    {
        if (_minimizationHookPointer == IntPtr.Zero) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWinEvent(_minimizationHookPointer);
            _minimizationHookPointer = IntPtr.Zero;
        }, DispatcherPriority.Normal);
    }

    private void OnMinimizeEndHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        OnMinimizeEnd?.Invoke(this, null);
    }

    public async Task<bool> AwaitMinimizationEndAsync(CancellationToken cancellationToken)
    {
        if (!_hookedGameDataProvider.Data!.GameWindowInfo!.GetMinimizationState()) return true;

        SetMinimizeEndHook((uint)_hookedGameDataProvider.Data!.GameProcess!.Id);

        _hookedGameDataProvider.SetStateAndNotify(EHookState.Hooked);

        _minimizationEndSemaphore = new SemaphoreSlim(0);
        OnMinimizeEnd += MinimizeEnd;

        if (!_hookedGameDataProvider.Data!.GameWindowInfo!.GetMinimizationState())
        {
            ReleaseMinimizationResources();
        }

        try
        {
            await _minimizationEndSemaphore?.WaitAsync(cancellationToken)!;
            _processInfoService.ApplyWindowInfo();
        }
        catch
        {
            return false;
        }

        return true;
    }

    private void MinimizeEnd(object sender, EventArgs e) => ReleaseMinimizationResources();

    private void ReleaseMinimizationResources()
    {
        OnMinimizeEnd -= MinimizeEnd;
        UnSetMinimizeEndHook();
        _minimizationEndSemaphore?.Release();
    }
    #endregion

    #region Location
    public void InitializeWindowLocationHook()
    {
        _targetWindowPointer = _hookedGameDataProvider.Data!.GameProcess!.MainWindowHandle;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _locationChangedHookPointer = NativeMethods.SetWinEventHook(
                NativeMethods.EVENT_SYSTEM_MOVESIZEEND, NativeMethods.EVENT_SYSTEM_MOVESIZEEND,
                IntPtr.Zero,
                _locationDelegate,
                (uint)_hookedGameDataProvider.Data!.GameProcess!.Id, 0,
                NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS | NativeMethods.WINEVENT_SKIPOWNTHREAD);
        }, DispatcherPriority.Normal);
    }

    private void UnSetLocationHook()
    {
        if (_locationChangedHookPointer == IntPtr.Zero) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWinEvent(_locationChangedHookPointer);
            _locationChangedHookPointer = IntPtr.Zero;
        }, DispatcherPriority.Normal);
    }

    private void LocationChangedHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (_targetWindowPointer == IntPtr.Zero) return;
        _processInfoService.SetWindowLocation();
        OnLocationChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Cleanup
    public void UnSetAllHooks()
    {
        UnSetFocusHook();
        UnSetLocationHook();
        UnSetMinimizeEndHook();
    }

    public void Dispose()
    {
        _minimizationEndSemaphore?.Dispose();
        _minimizationEndSemaphore = null;

        UnSetAllHooks();
        _targetWindowPointer = IntPtr.Zero;
        OnMinimizeEnd -= MinimizeEnd;
    }
    #endregion
}
