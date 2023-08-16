using System.Xml.Serialization;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

[XmlType("Key")]
[XmlInclude(typeof(KeyboardKeyDto))]
[XmlInclude(typeof(MouseKeyDto))]
public abstract record GenericKeyDto
{
    [XmlElement("Code")]
    public int KeyCode { get; set; }

    protected GenericKeyDto(int keyCode)
    {
        KeyCode = keyCode;
    }

    protected GenericKeyDto()
    {
    }
}
