using System;
using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.PeripheralEmulators;

namespace AuroraDialogEnhancer.Backend.ScriptHandlers;

public class ScriptHandlerService : IDisposable
{
    private readonly KeyboardEmulationService _keyboardEmulationService;
    private readonly MouseEmulationService    _mouseEmulationService;
    private readonly ModifierKeysProvider     _modifierKeysProvider;

    private readonly Dictionary<string, Action>         _scriptActionsDict;
    private readonly Dictionary<string, SplitKeyStruct> _keyActionsDict;

    public ScriptHandlerService(KeyboardEmulationService keyboardEmulationService,
                                MouseEmulationService    mouseEmulationService,
                                ModifierKeysProvider     modifierKeysProvider)
    {
        _keyboardEmulationService = keyboardEmulationService;
        _mouseEmulationService    = mouseEmulationService;
        _modifierKeysProvider     = modifierKeysProvider;

        _scriptActionsDict = new Dictionary<string, Action>();
        _keyActionsDict    = new Dictionary<string, SplitKeyStruct>();
    }

    public void RegisterAction(string id, List<GenericKey> keyList)
    {
        // Mouse
        if (keyList.Count == 1 && keyList[0].GetType() == typeof(MouseKey))
        {
            _scriptActionsDict.Add(id, () => _mouseEmulationService.DoMouseAction((EHighMouseKey)keyList[0].KeyCode));
            return;
        }

        // Keyboard
        var modifierKeys = new List<byte>();
        byte? regularKey = null;

        foreach (var keyboardKey in keyList)
        {
            if (_modifierKeysProvider.IsModifierKey(keyboardKey.KeyCode))
            {
                modifierKeys.Add((byte) keyboardKey.KeyCode);
                continue;
            }

            regularKey = (byte) keyboardKey.KeyCode;
        }

        var splitKeyStruct = new SplitKeyStruct(modifierKeys, regularKey ?? 0);
        _keyActionsDict.Add(id, splitKeyStruct);

        // Regular
        if (modifierKeys.Count == 0)
        {
            _scriptActionsDict.Add(id, () => _keyboardEmulationService.DoKeyboardClickOnlyRegular(_keyActionsDict[id]));
            return;
        }

        // Modifiers
        if (regularKey is null)
        {
            _scriptActionsDict.Add(id, () => _keyboardEmulationService.DoKeyboardClickModifiers(_keyActionsDict[id]));
            return;
        }

        // Modifiers + Regular
        _scriptActionsDict.Add(id, () => _keyboardEmulationService.DoKeyboardClickFull(_keyActionsDict[id]));
    }

    public void DoAction(string id) => _scriptActionsDict[id].Invoke();

    public void UnRegister(string id)
    {
        _keyActionsDict.Remove(id);
        _scriptActionsDict.Remove(id);
    }

    public void UnRegisterAll() => Dispose();

    public void Dispose()
    {
        _scriptActionsDict.Clear();
        _keyActionsDict.Clear();
    }
}
