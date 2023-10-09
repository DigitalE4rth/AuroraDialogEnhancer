using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.PeripheralEmulators;
using System;
using System.Collections.Generic;

namespace AuroraDialogEnhancer.Backend.ScriptHandlers;

public class AutoClickScript : IDisposable
{
    private readonly KeyboardEmulationService _keyboardEmulationService;
    private readonly MouseEmulationService    _mouseEmulationService;
    private readonly ModifierKeysProvider     _modifierKeysProvider;

    private Action?         _scriptAction;
    private SplitKeyStruct? _keyStruct;
    private EHighMouseKey   _mouseKey;

    public AutoClickScript(KeyboardEmulationService keyboardEmulationService,
                           MouseEmulationService    mouseEmulationService,
                           ModifierKeysProvider     modifierKeysProvider)
    {
        _keyboardEmulationService = keyboardEmulationService;
        _mouseEmulationService = mouseEmulationService;
        _modifierKeysProvider = modifierKeysProvider;
    }

    public void Register(List<GenericKey> keyList)
    {
        // Mouse
        if (keyList.Count == 1 && keyList[0].GetType() == typeof(MouseKey))
        {
            _mouseKey = (EHighMouseKey)keyList[0].KeyCode;
            _scriptAction = () => _mouseEmulationService.DoMouseAction(_mouseKey);
            return;
        }

        // Keyboard
        var modifierKeys = new List<byte>();
        byte? regularKey = null;

        foreach (var keyboardKey in keyList)
        {
            if (_modifierKeysProvider.IsModifierKey(keyboardKey.KeyCode))
            {
                modifierKeys.Add((byte)keyboardKey.KeyCode);
                continue;
            }

            regularKey = (byte)keyboardKey.KeyCode;
        }

        var splitKeyStruct = new SplitKeyStruct(modifierKeys, regularKey ?? 0);
        _keyStruct = splitKeyStruct;

        // Regular
        if (modifierKeys.Count == 0)
        {
            _scriptAction = () => _keyboardEmulationService.DoKeyboardClickOnlyRegular(_keyStruct);
            return;
        }

        // Modifiers
        if (regularKey is null)
        {
            _scriptAction = () => _keyboardEmulationService.DoKeyboardClickModifiers(_keyStruct);
            return;
        }

        // Modifiers + Regular
        _scriptAction = () => _keyboardEmulationService.DoKeyboardClickFull(_keyStruct);
    }

    public void DoAction() => _scriptAction!.Invoke();

    public void UnRegister()
    {
        _scriptAction = null;
        _keyStruct    = null;
    }

    public void Dispose() => UnRegister();
}
