using System;
using System.Collections.Generic;
using System.Linq;

namespace AuroraDialogEnhancer.Backend.Hooks.Keyboard;

public class KeyBindStruct : IEquatable<KeyBindStruct>
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

    public bool Equals(KeyBindStruct? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return VirtualKeys.SetEquals(other.VirtualKeys);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((KeyBindStruct)obj);
    }

    public override int GetHashCode() => VirtualKeys.Aggregate(17, (current, virtualKey) => (current * 7) + virtualKey.GetHashCode());
}
