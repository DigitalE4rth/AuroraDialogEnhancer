using System.Collections.Generic;
using System.Windows.Input;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;

public class KeyboardKeyInterpreterService
{
    private readonly Dictionary<int, string> _normalizedKeyNames = new()
    {
        { 192, "~"  }, { 189, "-" }, { 187, "="  },
        { 221, "]"  }, { 219, "[" }, { 220, "\\" },
        { 222, "\"" }, { 186, ";" },
        { 191, "?"  }, { 190, "." }, { 188, "," },
        { 48,  "0"  }, { 49,  "1" }, { 50, "2"  }, { 51, "3" }, { 52, "4" }, { 53, "5" }, { 54, "6" }, { 55, "7" }, { 56, "8" }, { 57, "9" }
    };
    
    public string GetAsString(int virtualKeyCode)
    {
        _normalizedKeyNames.TryGetValue(virtualKeyCode, out var result);
        return result ?? KeyInterop.KeyFromVirtualKey(virtualKeyCode).ToString();
    }
}
