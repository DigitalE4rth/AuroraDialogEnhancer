using System.Collections.Generic;
using System.Linq;

namespace AuroraDialogEnhancer.Backend.Hooks.Keyboard;

public class KeyBindStruct
{
    public readonly SortedSet<int> VirtualKeys;

    public KeyBindStruct(IEnumerable<int> modifierKeys, int regularKey)
    {
        VirtualKeys = new SortedSet<int>(modifierKeys) { regularKey };
    }

    public KeyBindStruct(IEnumerable<int> modifierKeys)
    {
        VirtualKeys = new SortedSet<int>(modifierKeys);
    }

    public KeyBindStruct()
    {
        VirtualKeys = new SortedSet<int>();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return VirtualKeys.SetEquals(((KeyBindStruct)obj).VirtualKeys);
    }

    public override int GetHashCode() => VirtualKeys.Aggregate(17, (total, next) => total * 7 + next.GetHashCode());
}
