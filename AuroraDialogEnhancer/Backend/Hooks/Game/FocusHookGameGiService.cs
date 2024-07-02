using System;
using AuroraDialogEnhancer.Backend.External;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using AuroraDialogEnhancer.Backend.Hooks.Window;
using AuroraDialogEnhancer.Backend.Hooks.WindowGi;

namespace AuroraDialogEnhancer.Backend.Hooks.Game;

public class FocusHookGameGiService : IGameFocusService
{
    private readonly FocusHookGiService        _focusHookGiService;
    private readonly KeyboardFocusHookService  _keyboardFocusHookService;
    private readonly MinimizationHookGiService _minimizationHookGiService;
    private readonly ProcessDataProvider       _processDataProvider;
    private readonly ProcessInfoService        _processInfoService;

    private readonly object _lock = new {};
    private int  _foregroundCurrentId;
    private bool _isMinimized;
    private bool _isForegroundPrevious;

    public bool IsFocused { get; private set; }

    public FocusHookGameGiService(FocusHookGiService        focusHookGiService,
                                  KeyboardFocusHookService  keyboardFocusHookService,
                                  MinimizationHookGiService minimizationHookGiService,
                                  ProcessDataProvider       processDataProvider,
                                  ProcessInfoService        processInfoService)
    {
        _focusHookGiService        = focusHookGiService;
        _keyboardFocusHookService  = keyboardFocusHookService;
        _minimizationHookGiService = minimizationHookGiService;
        _processDataProvider       = processDataProvider;
        _processInfoService        = processInfoService;
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
        _focusHookGiService.SetWinEventHook(FocusEventHookCallback);
        IsFocused = _focusHookGiService.IsTargetWindowForeground();
        DetectFocusAndSendEvent();

        _minimizationHookGiService.SetWinEventHook(MinimizationEventHookCallback);
        _isMinimized = _processDataProvider.Data!.GameWindowInfo!.IsMinimized();
        DetectFocusAndSendEvent();
        
        _keyboardFocusHookService.SetWinEventHook(KeyboardFocusEventHookCallback);
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
        _isMinimized = eventType == _minimizationHookGiService.EventMin;
        DetectFocusAndSendEvent();
    }
    
    private void KeyboardFocusEventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (hwnd == IntPtr.Zero) return;
     
        NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), out var foregroundId);
        
        if (foregroundId == _processDataProvider.Data!.GameProcess!.Id && foregroundId == _foregroundCurrentId) return;
        _keyboardFocusHookService.UnhookWinEvent();

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
        _focusHookGiService.UnhookWinEvent();
        _minimizationHookGiService.UnhookWinEvent();
        _keyboardFocusHookService.UnhookWinEvent();
    }
}
