using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancerExtensions.KeyBinding;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;

public class KeyInterpreterService
{
    private readonly KeyboardKeyInterpreterService _keyboardKeyInterpreterService;
    private readonly MouseKeyInterpreterService    _mouseKeyInterpreterService;

    public KeyInterpreterService(KeyboardKeyInterpreterService keyboardKeyInterpreterService,
                                 MouseKeyInterpreterService    mouseKeyInterpreterService)
    {
        _keyboardKeyInterpreterService = keyboardKeyInterpreterService;
        _mouseKeyInterpreterService    = mouseKeyInterpreterService;
    }

    public List<string> InterpretKeys(List<GenericKey> rawKeys)
    {
        return rawKeys.Select(key => key.GetType() == typeof(KeyboardKey) 
            ? _keyboardKeyInterpreterService.GetAsString(key.KeyCode) 
            : _mouseKeyInterpreterService.GetAsString((EHighMouseKey)key.KeyCode))
            .ToList();
    }

    public string InterpretKey(GenericKey key)
    {
        return key.GetType() == typeof(KeyboardKey)
            ? _keyboardKeyInterpreterService.GetAsString(key.KeyCode)
            : _mouseKeyInterpreterService.GetAsString((EHighMouseKey) key.KeyCode);
    }

    public string InterpretKey(int keyboardKeyCode)
    {
        return _keyboardKeyInterpreterService.GetAsString(keyboardKeyCode);
    }

    public string InterpretKey(EHighMouseKey mouseKeyCode)
    {
        return _mouseKeyInterpreterService.GetAsString(mouseKeyCode);
    }

    public string GetAsString(IEnumerable<GenericKey> keyBindStruct)
    {
        var interpretedKeys = keyBindStruct.Select(genericKey => genericKey.GetType() == typeof(KeyboardKey)
            ? _keyboardKeyInterpreterService.GetAsString(genericKey.KeyCode)
            : _mouseKeyInterpreterService.GetAsString((EHighMouseKey)genericKey.KeyCode));

        return string.Join("+", interpretedKeys);
    }

    public string GetAsJoinedString(IEnumerable<IEnumerable<GenericKey>> keyBindStruct) =>
        string.Join(", ", keyBindStruct.Select(GetAsString));
}
