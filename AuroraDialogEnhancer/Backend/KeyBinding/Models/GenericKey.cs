using System;
using System.Xml.Serialization;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

[XmlType("Key")]
[XmlInclude(typeof(KeyboardKey))]
[XmlInclude(typeof(MouseKey))]
public abstract class GenericKey : IEquatable<GenericKey>
{
    protected GenericKey()
    {
    }

    protected GenericKey(int keyCode)
    {
        KeyCode = keyCode;
    }

    [XmlElement("Code")]
    public int KeyCode { get; set; }

    public bool Equals(GenericKey? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return KeyCode == other.KeyCode;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((GenericKey)obj);
    }

    public override int GetHashCode()
    {
        return KeyCode;
    }
}
