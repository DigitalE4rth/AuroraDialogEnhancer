using System;
using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyHandler.Scripts;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class KeyActionRegistrar : IDisposable
{
    private readonly KeyActionExecution         _keyActionExecution;
    private readonly KeyActionUtility           _keyActionUtility;
    private readonly KeyPauseActionProvider     _keyPauseActionProvider;
    private readonly KeyboardHookManagerService _keyboardHookManagerService;
    private readonly MouseHookManagerService    _mouseHookManagerService;
    private readonly ScriptAutoSkip             _scriptAutoSkip;
    private readonly ScriptAutoClick            _scriptAutoClick;

    public KeyActionRegistrar(KeyActionExecution          keyActionExecution,
                              KeyActionUtility            keyActionUtility,
                              KeyPauseActionProvider      keyPauseActionProvider,
                              KeyboardHookManagerService  keyboardHookManagerService,
                              MouseHookManagerService     mouseHookManagerService,
                              ScriptAutoSkip              scriptAutoSkip,
                              ScriptAutoClick             scriptAutoClick)
    {
        _keyActionExecution         = keyActionExecution;
        _keyActionUtility           = keyActionUtility;
        _keyPauseActionProvider     = keyPauseActionProvider;
        _keyboardHookManagerService = keyboardHookManagerService;
        _mouseHookManagerService    = mouseHookManagerService;
        _scriptAutoSkip             = scriptAutoSkip;
        _scriptAutoClick            = scriptAutoClick;

        _keyPauseActionProvider.Initialize(this);
    }

    public void Register(List<List<GenericKey>> keysOfTriggers, Action action)
    {
        foreach (var genericKeys in keysOfTriggers)
        {
            if (genericKeys.Count == 1 && genericKeys[0].GetType() == typeof(MouseKey))
            {
                _mouseHookManagerService.RegisterHotKey(genericKeys[0].KeyCode, action);
                continue;
            }

            _keyboardHookManagerService.RegisterHotKeys(genericKeys.Select(key => key.KeyCode), action);
        }
    }

    public void Register(List<InteractionPoint> interactionPoints)
    {
        foreach (var interactionPoint in interactionPoints)
        {
            foreach (var activationKeys in interactionPoint.ActivationKeys)
            {
                if (activationKeys.Count == 1 && activationKeys[0].GetType() == typeof(MouseKey))
                {
                    _mouseHookManagerService.RegisterHotKey(
                        activationKeys[0].KeyCode,
                        () => { _keyActionExecution.ClickPrimaryMouseButtonWithAccess(_keyActionUtility.InteractionPoints[interactionPoint.Id]); }
                    );
                    continue;
                }

                _keyboardHookManagerService.RegisterHotKeys(
                    activationKeys.Select(key => key.KeyCode),
                    () => { _keyActionExecution.ClickPrimaryMouseButtonWithAccess(_keyActionUtility.InteractionPoints[interactionPoint.Id]); }
                );
            }
        }
    }

    public void Register(List<InteractionPrecisePoint> interactionPoints)
    {
        _keyActionUtility.InteractionPoints = interactionPoints.ToDictionary(point => point.Id, point => point.Point);
    }

    public void UnRegister(List<List<GenericKey>> keysOfTriggers)
    {
        foreach (var genericKeys in keysOfTriggers)
        {
            if (genericKeys.Count == 1 && genericKeys[0].GetType() == typeof(MouseKey))
            {
                _mouseHookManagerService.UnRegisterHotKey(genericKeys[0].KeyCode);
                continue;
            }

            _keyboardHookManagerService.UnRegisterHotKeys(genericKeys.Select(key => key.KeyCode));
        }
    }

    public void RegisterKeyBinds()
    {
        Register(_keyActionUtility.KeyBindingProfile.PauseResume, _keyPauseActionProvider.OnPauseSwitch);
        Register(_keyActionUtility.KeyBindingProfile.Reload,      _keyActionExecution.OnReload);
        Register(_keyActionUtility.KeyBindingProfile.Screenshot,  _keyActionExecution.OnScreenshot);
        Register(_keyActionUtility.KeyBindingProfile.HideCursor,  _keyActionExecution.OnHideCursor);

        Register(_keyActionUtility.KeyBindingProfile.Select,   _keyActionExecution.OnSelectPress);
        Register(_keyActionUtility.KeyBindingProfile.Next,     _keyActionExecution.OnNextPress);
        Register(_keyActionUtility.KeyBindingProfile.Previous, _keyActionExecution.OnPreviousPress);
        Register(_keyActionUtility.KeyBindingProfile.Last,     _keyActionExecution.OnLastPress);

        Register(_keyActionUtility.KeyBindingProfile.One,   _keyActionExecution.OnOnePress);
        Register(_keyActionUtility.KeyBindingProfile.Two,   _keyActionExecution.OnTwoPress);
        Register(_keyActionUtility.KeyBindingProfile.Three, _keyActionExecution.OnThreePress);
        Register(_keyActionUtility.KeyBindingProfile.Four,  _keyActionExecution.OnFourPress);
        Register(_keyActionUtility.KeyBindingProfile.Five,  _keyActionExecution.OnFivePress);
        Register(_keyActionUtility.KeyBindingProfile.Six,   _keyActionExecution.OnSixPress);
        Register(_keyActionUtility.KeyBindingProfile.Seven, _keyActionExecution.OnSevenPress);
        Register(_keyActionUtility.KeyBindingProfile.Eight, _keyActionExecution.OnEightPress);
        Register(_keyActionUtility.KeyBindingProfile.Nine,  _keyActionExecution.OnNinePress);
        Register(_keyActionUtility.KeyBindingProfile.Ten,   _keyActionExecution.OnTenPress);

        Register(_keyActionUtility.KeyBindingProfile.InteractionPoints);
        RegisterAutoSkip();

        _mouseHookManagerService.RegisterPrimaryClick(_keyActionExecution.OnMousePrimaryClick);
    }

    public void RegisterAutoSkip()
    {
        _scriptAutoClick.Register(_keyActionUtility.KeyBindingProfile.AutoSkipConfig.SkipKeys);
        Register(_keyActionUtility.KeyBindingProfile.AutoSkipConfig.ActivationKeys, _scriptAutoSkip.OnAutoSkip);
        _scriptAutoSkip.ApplySettings();
    }

    public void UnRegisterAll()
    {
        _mouseHookManagerService.UnRegisterAll();
        _keyboardHookManagerService.UnRegisterAll();
        _scriptAutoClick.UnRegister();
    }

    public void Dispose()
    {
        UnRegisterAll();
    }
}
