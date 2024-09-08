using System;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyHandler.Scripts;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class KeyActionControls : IDisposable
{
    private readonly FocusHookService            _focusHookService;
    private readonly KeyActionRegistrar          _keyActionRegistrar;
    private readonly KeyActionUtility            _keyActionUtility;
    private readonly KeyPauseActionProvider      _keyPauseActionProvider;
    private readonly KeyboardHookManagerService  _keyboardHookManagerService;
    private readonly MouseHookManagerService     _mouseHookManagerService;
    private readonly ProcessDataProvider         _processDataProvider;
    private readonly ScriptAutoClick             _scriptAutoClick;
    private readonly ScriptAutoSkip              _scriptAutoSkip;

    public KeyActionControls(FocusHookService            focusHookService,
                             KeyActionRegistrar          keyActionRegistrar,
                             KeyActionUtility            keyActionUtility,
                             KeyPauseActionProvider      keyPauseActionProvider,
                             KeyboardHookManagerService  keyboardHookManagerService,
                             MouseHookManagerService     mouseHookManagerService,
                             ProcessDataProvider         processDataProvider,
                             ScriptAutoClick             scriptAutoClick,
                             ScriptAutoSkip              scriptAutoSkip)
    {
        _focusHookService           = focusHookService;
        _keyActionRegistrar         = keyActionRegistrar;
        _keyActionUtility           = keyActionUtility;
        _keyPauseActionProvider     = keyPauseActionProvider;
        _keyboardHookManagerService = keyboardHookManagerService;
        _mouseHookManagerService    = mouseHookManagerService;
        _processDataProvider        = processDataProvider;
        _scriptAutoClick            = scriptAutoClick;
        _scriptAutoSkip             = scriptAutoSkip;
    }

    public void ApplyKeyBinds()
    {
        StopPeripheryHook();
        _keyboardHookManagerService.UnRegisterAll();
        _mouseHookManagerService.UnRegisterAll();
        _scriptAutoClick.UnRegister();
        _keyActionUtility.InitializeProfile();
        _keyActionRegistrar.RegisterKeyBinds();
        StartPeripheryHook();
    }

    public void StartPeripheryHook()
    {
        _keyboardHookManagerService.Start();
        _mouseHookManagerService.Start();
    }

    public void StopPeripheryHook()
    {
        _scriptAutoSkip.Stop();
        _keyboardHookManagerService.Stop();
        _mouseHookManagerService.Stop();
        _keyActionUtility.DialogOptions.Clear();
    }

    public bool IsResumePause()
    {
        if (_processDataProvider.HookState is not EHookState.Paused) return false;

        _keyPauseActionProvider.OnPauseSwitch();
        return true;
    }

    public void InitializeFocusHook()
    {
        _focusHookService.OnFocusChanged -= ApplicationFocusChanged;
        _focusHookService.Initialize();
        _focusHookService.OnFocusChanged += ApplicationFocusChanged;
    }

    private void ApplicationFocusChanged(object sender, bool state)
    {
        if (state)
        {
            StartPeripheryHook();
            return;
        }

        StopPeripheryHook();
    }

    public void Dispose()
    {
        _focusHookService.OnFocusChanged -= ApplicationFocusChanged;
        _focusHookService.Dispose();
        StopPeripheryHook();
    }
}
