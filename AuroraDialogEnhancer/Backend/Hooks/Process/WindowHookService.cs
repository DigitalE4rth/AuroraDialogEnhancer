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
    private NativeMethods.WinEventDelegate? _focusDelegate;

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

        _minimizeEndDelegate = OnMinimizeEndHook;
        _locationDelegate    = LocationChangedHook;
    }

    #region Focus
    private bool IsTargetWindowForeground()
    {
        var activatedHandle = NativeMethods.GetForegroundWindow();
        if (activatedHandle == IntPtr.Zero)
        {
            return false;
        }

        if (_hookedGameDataProvider.Data?.GameProcess is null) return false;

        var procId = _hookedGameDataProvider.Data.GameProcess.Id;
        NativeMethods.GetWindowThreadProcessId(activatedHandle, out var activeProcId);

        return activeProcId == procId;
    }

    /// <summary>
    /// Sets up target window focus hook.
    /// </summary>
    /// <remarks>
    /// If, after launching the exe file of the game, focus on any other window, then after the game window is displayed,
    /// with subsequent focus on the game window using the taskbar, the hook says that the focus gets explorer and not the game.
    /// And only the subsequent refocusing, by pressing the alt-tab buttons twice, gives the hook information that the game has received focus.
    /// <br/><br/>
    /// I was unable to fully understand the reasons for this behavior,
    /// so I increased the initial range of hook messages in order to receive messages not only about focusing,
    /// but also about whether the window captured the mouse cursor.
    /// <br/><br/>
    /// And only after determining that the game has really received focus, you can reduce the range of messages,
    /// because the mouse cursor capture event reports every click on the mouse buttons.
    /// </remarks>
    public void InitializeFocusHook()
    {
        _focusDelegate = InitialFocusHook;
        IsTargetWindowFocused = IsTargetWindowForeground();

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _focusChangedHookPointer = NativeMethods.SetWinEventHook(NativeMethods.EVENT_SYSTEM_FOREGROUND, NativeMethods.EVENT_SYSTEM_CAPTURESTART, IntPtr.Zero, _focusDelegate, 0, 0, NativeMethods.WINEVENT_OUTOFCONTEXT);
        }, DispatcherPriority.Normal);

        OnFocusChanged?.Invoke(this, IsTargetWindowFocused);
    }

    public void SendFocusedEvent()
    {
        IsTargetWindowFocused = IsTargetWindowForeground();
        OnFocusChanged?.Invoke(this, IsTargetWindowFocused);
    }

    /// <summary>
    /// Un-sets target window focus hook.
    /// </summary>
    private void UnSetFocusHook()
    {
        IsTargetWindowFocused = false;

        if (_focusChangedHookPointer == IntPtr.Zero) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWinEvent(_focusChangedHookPointer);
            _focusChangedHookPointer = IntPtr.Zero;
        }, DispatcherPriority.Normal);
    }

    /// <summary>
    /// Focus changed delegate.
    /// </summary>
    /// <remarks>See <see cref="NativeMethods.SetWinEventHook"/></remarks>
    private void InitialFocusHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (IsTargetWindowFocused && IsTargetWindowForeground() == false)
        {
            IsTargetWindowFocused = false;
            OnFocusChanged?.Invoke(this, false);
            return;
        }

        if (IsTargetWindowFocused == false && IsTargetWindowForeground() == false) return;

        UnSetFocusHook();
        IsTargetWindowFocused = true;
        _focusDelegate = FocusChangedHook;
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _focusChangedHookPointer = NativeMethods.SetWinEventHook(NativeMethods.EVENT_SYSTEM_FOREGROUND, NativeMethods.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _focusDelegate, 0, 0, NativeMethods.WINEVENT_OUTOFCONTEXT);
        }, DispatcherPriority.Normal);

        OnFocusChanged?.Invoke(this, true);
    }

    /// <summary>
    /// Focus changed delegate.
    /// </summary>
    /// <remarks>See <see cref="NativeMethods.SetWinEventHook"/></remarks>
    private void FocusChangedHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        var isForeground = IsTargetWindowForeground();

        switch (IsTargetWindowFocused)
        {
            case true when !isForeground:
                OnFocusChanged?.Invoke(null, false);
                IsTargetWindowFocused = false;
                return;
            case false when isForeground:
                OnFocusChanged?.Invoke(null, true);
                IsTargetWindowFocused = true;
                break;
        }
    }
    #endregion


    #region Location
    public void InitializeWindowLocationHook()
    {
        SetLocationHook((uint)_hookedGameDataProvider.Data!.GameProcess!.Id);
    }

    /// <summary>
    /// Sets up target window location hook.
    /// </summary>
    private void SetLocationHook(uint processId)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _locationChangedHookPointer = NativeMethods.SetWinEventHook(NativeMethods.EVENT_SYSTEM_MOVESIZEEND,
                NativeMethods.EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, _locationDelegate, processId, 0,
                NativeMethods.WINEVENT_OUTOFCONTEXT);
        }, DispatcherPriority.Normal);
    }

    /// <summary>
    /// Un-sets target window location hook.
    /// </summary>
    private void UnSetLocationHook()
    {
        if (_locationChangedHookPointer == IntPtr.Zero) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            NativeMethods.UnhookWinEvent(_locationChangedHookPointer);
            _locationChangedHookPointer = IntPtr.Zero;
        }, DispatcherPriority.Normal);
    }

    /// <summary>
    /// Location changed delegate.
    /// </summary>
    /// <remarks>See <see cref="NativeMethods.SetWinEventHook"/></remarks>
    private void LocationChangedHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (_targetWindowPointer == IntPtr.Zero) return;
        _processInfoService.SetWindowLocation();
        OnLocationChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion


    #region Minimization
    /// <summary>
    /// Sets up target window minimization end hook.
    /// </summary>
    private void SetMinimizeEndHook(uint processId)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _minimizeEndHookPointer = NativeMethods.SetWinEventHook(NativeMethods.EVENT_SYSTEM_MINIMIZEEND,
                NativeMethods.EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, _minimizeEndDelegate, processId, 0,
                NativeMethods.WINEVENT_OUTOFCONTEXT);
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

    /// <summary>
    /// Un-sets target window minimization end hook.
    /// </summary>
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

    /// <summary>
    /// Captures minimization end event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event args.</param>
    private void MinimizeEnd(object sender, EventArgs e) => ReleaseMinimizationResources();

    /// <summary>
    /// Releases minimization resources and <see cref="_minimizationEndSemaphore"/>.
    /// </summary>
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

    /// <summary>
    /// Cleans up resources.
    /// </summary>
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
