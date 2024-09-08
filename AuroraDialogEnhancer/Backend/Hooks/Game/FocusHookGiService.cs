using System;
using AuroraDialogEnhancer.Backend.External;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using AuroraDialogEnhancer.Backend.Hooks.Window;
using AuroraDialogEnhancer.Backend.Hooks.WindowGi;

namespace AuroraDialogEnhancer.Backend.Hooks.Game;

public class FocusHookGiService : IApplicationFocusService
{
    private readonly FocusHookGi         _focusHookGi;
    private readonly KeyboardFocusHook   _keyboardFocusHook;
    private readonly MinimizationHookGi  _minimizationHookGi;
    private readonly ProcessDataProvider _processDataProvider;
    private readonly ProcessInfoService  _processInfoService;

    private readonly object _lock = new {};
    private int  _foregroundCurrentId;
    private bool _isMinimized;
    private bool _isForegroundPrevious;

    public bool IsFocused { get; private set; }

    public FocusHookGiService(FocusHookGi         focusHookGi,
                              KeyboardFocusHook   keyboardFocusHook,
                              MinimizationHookGi  minimizationHookGi,
                              ProcessDataProvider processDataProvider,
                              ProcessInfoService  processInfoService)
    {
        _focusHookGi         = focusHookGi;
        _keyboardFocusHook   = keyboardFocusHook;
        _minimizationHookGi  = minimizationHookGi;
        _processDataProvider = processDataProvider;
        _processInfoService  = processInfoService;
    }
    
    public event EventHandler<bool>? OnFocusChanged;
    
    private void DetectFocusAndSendEvent()
    {
        lock (_lock)
        {
            _isForegroundPrevious = IsFocused;
            IsFocused  = _foregroundCurrentId == _processDataProvider.Data!.GameProcess!.Id;
            
            if (_isForegroundPrevious == IsFocused) return;
            if (IsFocused && _isMinimized)
            {
                IsFocused = false;
                return;
            }

            OnFocusChanged?.Invoke(this, IsFocused);
        }
    }

    public void SetWinEventHook()
    {
        _focusHookGi.SetWinEventHook(FocusEventHookCallback);
        IsFocused = _focusHookGi.IsTargetWindowForeground();
        DetectFocusAndSendEvent();

        _minimizationHookGi.SetWinEventHook(MinimizationEventHookCallback);
        _isMinimized = _processDataProvider.Data!.GameWindowInfo!.IsMinimized();
        DetectFocusAndSendEvent();
        
        _keyboardFocusHook.SetWinEventHook(KeyboardFocusEventHookCallback);
    }

    public void SendFocusedEvent()
    {
        NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), out _foregroundCurrentId);
        IsFocused = _foregroundCurrentId == _processDataProvider.Data!.GameProcess!.Id;
        OnFocusChanged?.Invoke(this, IsFocused);
    }

    private void FocusEventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        NativeMethods.GetWindowThreadProcessId(hwnd, out _foregroundCurrentId);
        DetectFocusAndSendEvent();
    }
    
    private void MinimizationEventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        _isMinimized = eventType == _minimizationHookGi.EventMin;
        DetectFocusAndSendEvent();
    }
    
    private void KeyboardFocusEventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (hwnd == IntPtr.Zero) return;
     
        NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), out var foregroundId);
        
        if (foregroundId == _processDataProvider.Data!.GameProcess!.Id && foregroundId == _foregroundCurrentId) return;
        _keyboardFocusHook.UnhookWinEvent();

        var foregroundWindow = NativeMethods.GetForegroundWindow();
        NativeMethods.SetForegroundWindow(_processDataProvider.Data!.GameProcess!.MainWindowHandle);
        NativeMethods.SetForegroundWindow(foregroundWindow);
        
        _processInfoService.ApplyWindowInfo();
        _processInfoService.SetWindowLocation();

        _foregroundCurrentId = foregroundId;
        
        DetectFocusAndSendEvent();
    }

    public void UnhookWinEvent()
    {
        _focusHookGi.UnhookWinEvent();
        _minimizationHookGi.UnhookWinEvent();
        _keyboardFocusHook.UnhookWinEvent();
    }
}
