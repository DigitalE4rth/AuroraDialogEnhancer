using System;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.External;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Backend.Hooks.Global;

public class GlobalFocusService : IFocusHook
{
    private readonly ExtensionConfigService _extensionConfigService;
    private readonly GlobalKeyboardHook     _keyboardHook;
    private readonly ProcessInfoService     _processInfoService;
    private readonly ProcessDataProvider    _processDataProvider;
    
    private bool _isStarted;
    public bool  IsFocused { get; private set; }
    public event EventHandler<bool>? OnFocusChanged;

    public GlobalFocusService(ExtensionConfigService extensionConfigService,
                              GlobalKeyboardHook     keyboardHook,
                              ProcessInfoService     processInfoService,
                              ProcessDataProvider    processDataProvider)
    {
        _extensionConfigService = extensionConfigService;
        _keyboardHook           = keyboardHook;
        _processInfoService     = processInfoService;
        _processDataProvider    = processDataProvider;
    }
    
    public void SetWinEventHook()
    {
        if (_isStarted) return;
        _keyboardHook.SetWinEventHook(KeyboardDelegate);
        _isStarted = true;
    }
    
    private void KeyboardDelegate(IntPtr hwineventhook, uint eventtype, IntPtr hwnd, int idobject, int idchild, uint dweventthread, uint dwmseventtime)
    {
        WinApi.GetWindowThreadProcessId(hwnd, out var keyboardFocusProcessId);
        var keyboardFocusProcess = _processInfoService.GetProcess(keyboardFocusProcessId);
        if (keyboardFocusProcess is null)
        {
            SetFocusAndSendEventIfDifferent(false);
            return;
        }

        var keyboardExtensionId = _extensionConfigService.GetIdByProcessName(keyboardFocusProcess.ProcessName);
        if (keyboardExtensionId is null)
        {
            SetFocusAndSendEventIfDifferent(false);
            return;
        }
        
        var foregroundPointer = WinApi.GetForegroundWindow();
        WinApi.GetWindowThreadProcessId(foregroundPointer, out var foregroundProcessId);
        
        
        // Problem Summary:
        //   The keyboard focus pointer targets an existing game window, while the foreground pointer does not.
        //   When launching a game or game world, the keyboard focus and window focus may be different.
        //   In addition, in this case, if the game is in windowed mode, then by clicking on the taskbar icon - the focused window becomes explorer, although the actual focus is on the game.
        // Solution Hack:
        //   For the actual information about the focused application, it is necessary to forcibly reapply focus
        if (keyboardFocusProcessId != foregroundProcessId)
        {
            var foregroundFocusProcess = _processInfoService.GetProcess(foregroundProcessId);
            if (foregroundFocusProcess is null)
            {
                WinApi.SetForegroundWindow(hwnd);
                SetFocusAndSendEventIfDifferent(true);
                return;
            }
            
            var isIconic = WinApi.IsIconic(hwnd);
            WinApi.SetForegroundWindow(hwnd);
            if (isIconic) WinApi.ShowWindowAsync(hwnd, EShowWindowMode.SW_FORCEMINIMIZE);
            WinApi.SetForegroundWindow(foregroundPointer);
            
            var foregroundExtensionId = _extensionConfigService.GetIdByProcessName(foregroundFocusProcess.ProcessName);
            if (foregroundExtensionId is not null)
            {
                AppServices.ServiceProvider.GetRequiredService<CoreService>().Run(foregroundExtensionId, EStartMode.StartOnly);
            }
            
            return;
        }
        
        if (_processDataProvider.Data?.GameProcess?.Id is not null && 
            keyboardFocusProcessId != _processDataProvider.Data.GameProcess.Id)
        {
            AppServices.ServiceProvider.GetRequiredService<CoreService>().Run(keyboardExtensionId, EStartMode.StartOnly);
            return;
        }
        
        SetFocusAndSendEventIfDifferent(true);
    }

    private void SetFocusAndSendEventIfDifferent(bool isFocused)
    {
        if (IsFocused == isFocused) return;
        IsFocused = isFocused;
        OnFocusChanged?.Invoke(this, IsFocused);
    }
    
    public void SendFocusedEvent()
    {
        var foregroundPointer = WinApi.GetForegroundWindow();
        WinApi.GetWindowThreadProcessId(foregroundPointer, out var foregroundProcessId);
        IsFocused = foregroundProcessId == _processDataProvider.Data!.GameProcess!.Id;
        OnFocusChanged?.Invoke(this, IsFocused);
    }

    public void UnhookWinEvent()
    {
        _keyboardHook.UnhookWinEvent();
        _isStarted = false;
    }
}
