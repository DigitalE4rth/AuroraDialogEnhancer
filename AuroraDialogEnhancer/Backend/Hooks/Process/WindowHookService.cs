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

    #region User32 resources
    /// <summary>
    /// Delegate of minimization end hook.
    /// </summary>
    private readonly NativeMethods.WinEventDelegate _minimizeEndDelegate;

    /// <summary>
    /// Delegate of location change hook.
    /// </summary>
    private readonly NativeMethods.WinEventDelegate _locationDelegate;

    /// <summary>
    /// Delegate of focus change hook.
    /// </summary>
    private NativeMethods.WinEventDelegate _focusDelegate;

    /// <summary>
    /// Target window pointer.
    /// </summary>
    private IntPtr _targetWindowPointer;

    /// <summary>
    /// Pointer of the change focus event.
    /// </summary>
    private IntPtr _focusChangedHookPointer;

    /// <summary>
    /// Pointer of the location change event.
    /// </summary>
    private IntPtr _locationChangedHookPointer;

    /// <summary>
    /// Pointer of the minimization end event.
    /// </summary>
    private IntPtr _minimizeEndHookPointer;
    #endregion

    #region Event handlers
    public event EventHandler<bool>? OnFocusChanged;
    public event EventHandler? OnMinimizeEnd;
    public event EventHandler? OnLocationChanged;
    #endregion

    /// <summary>
    /// Semaphore for capturing <see cref="OnMinimizeEnd"/> event.
    /// </summary>
    private SemaphoreSlim? _minimizationEndSemaphore;

    private bool IsTargetWindowFocused { get; set; }

    public WindowHookService(ProcessInfoService     processInfoService,
                             HookedGameDataProvider hookedGameDataProvider)
    {
        _processInfoService     = processInfoService;
        _hookedGameDataProvider = hookedGameDataProvider;

        _focusDelegate       = InitialFocusHook;
        _minimizeEndDelegate = OnMinimizeEndHook;
        _locationDelegate    = LocationChangedHook;
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
        // Tracking the event focusing the window by mouse click helps somewhat with this
        // bug if Unity game is in windowed mode
        // https://github.com/DigitalE4rth/AuroraDialogEnhancer/issues/9

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _focusChangedHookPointer = NativeMethods.SetWinEventHook(
                NativeMethods.EVENT_SYSTEM_FOREGROUND, 
                NativeMethods.EVENT_SYSTEM_CAPTURESTART, 
                IntPtr.Zero, 
                _focusDelegate, 
                0, 
                0, 
                NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS | NativeMethods.WINEVENT_SKIPOWNTHREAD);
        }, DispatcherPriority.Normal);

        IsTargetWindowFocused = IsTargetWindowForeground(NativeMethods.GetForegroundWindow());

        OnFocusChanged?.Invoke(this, IsTargetWindowFocused);
    }

    private void InitialFocusHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (IsTargetWindowFocused && IsTargetWindowForeground(hwnd) == false)
        {
            IsTargetWindowFocused = false;
            OnFocusChanged?.Invoke(this, false);
            return;
        }

        if (IsTargetWindowFocused == false && IsTargetWindowForeground(hwnd) == false) return;

        UnSetFocusHook();
        _focusDelegate = FocusChangedHook;
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _focusChangedHookPointer = NativeMethods.SetWinEventHook(
                NativeMethods.EVENT_SYSTEM_FOREGROUND,
                NativeMethods.EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, 
                _focusDelegate,
                0,
                0,
                NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS | NativeMethods.WINEVENT_SKIPOWNTHREAD);
        }, DispatcherPriority.Normal);

        IsTargetWindowFocused = true;
        OnFocusChanged?.Invoke(this, true);
    }

    private void FocusChangedHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        var isForeground = IsTargetWindowForeground(hwnd);

        switch (IsTargetWindowFocused)
        {
            case true when !isForeground:
                IsTargetWindowFocused = false;
                OnFocusChanged?.Invoke(null, false);
                return;
            case false when isForeground:
                IsTargetWindowFocused = true;
                OnFocusChanged?.Invoke(null, true);
                break;
        }
    }

    public void SendFocusedEvent()
    {
        IsTargetWindowFocused = IsTargetWindowForeground();
        OnFocusChanged?.Invoke(this, IsTargetWindowFocused);
    }

    private void UnSetFocusHook()
    {
        IsTargetWindowFocused = false;

        if (_focusChangedHookPointer == IntPtr.Zero) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWinEvent(_focusChangedHookPointer);
            _focusChangedHookPointer = IntPtr.Zero;
        }, DispatcherPriority.Normal);

        // Ensure
        IsTargetWindowFocused = false;
    }
    #endregion


    #region Location
    public void InitializeWindowLocationHook()
    {
        SetLocationHook((uint)_hookedGameDataProvider.Data!.GameProcess!.Id);
    }

    private void SetLocationHook(uint processId)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _locationChangedHookPointer = NativeMethods.SetWinEventHook(
                NativeMethods.EVENT_SYSTEM_MOVESIZEEND,
                NativeMethods.EVENT_SYSTEM_MOVESIZEEND, 
                IntPtr.Zero, 
                _locationDelegate, 
                processId, 
                0,
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


    #region Minimization
    private void SetMinimizeEndHook(uint processId)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _minimizeEndHookPointer = NativeMethods.SetWinEventHook(
                NativeMethods.EVENT_SYSTEM_MINIMIZEEND,
                NativeMethods.EVENT_SYSTEM_MINIMIZEEND, 
                IntPtr.Zero, 
                _minimizeEndDelegate, 
                processId, 
                0,
                NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS | NativeMethods.WINEVENT_SKIPOWNTHREAD);
        }, DispatcherPriority.Normal);
    }

    public void UnSetMinimizeEndHook()
    {
        if (_minimizeEndHookPointer == IntPtr.Zero) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWinEvent(_minimizeEndHookPointer);
            _minimizeEndHookPointer = IntPtr.Zero;
        }, DispatcherPriority.Normal);
    }

    private void OnMinimizeEndHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) => OnMinimizeEnd?.Invoke(this, null);

    public async Task<bool> AwaitMinimizationEndAsync(CancellationToken cancellationToken)
    {
        if (!_hookedGameDataProvider.Data!.GameWindowInfo!.IsMinimized()) return true;

        SetMinimizeEndHook((uint)_hookedGameDataProvider.Data!.GameProcess!.Id);

        _hookedGameDataProvider.SetStateAndNotify(EHookState.Hooked);

        _minimizationEndSemaphore = new SemaphoreSlim(0);
        OnMinimizeEnd += MinimizeEnd;

        if (!_hookedGameDataProvider.Data!.GameWindowInfo!.IsMinimized())
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
