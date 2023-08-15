using System;
using System.Xml.Serialization;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

[XmlType("Key")]
[XmlInclude(typeof(KeyboardKeyDto))]
[XmlInclude(typeof(MouseKeyDto))]
public abstract class GenericKeyDto : IEquatable<GenericKeyDto>
{
    protected GenericKeyDto()
    {
    }

    protected GenericKeyDto(int keyCode)
    {
        KeyCode = keyCode;
    }

    [XmlElement("Code")]
    public int KeyCode { get; set; }

    public bool Equals(GenericKeyDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return KeyCode == other.KeyCode;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((GenericKeyDto)obj);
    }

    public override int GetHashCode()
    {
        return KeyCode;
    }
}
