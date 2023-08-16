using System.Xml.Serialization;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

[XmlType("Key")]
[XmlInclude(typeof(KeyboardKey))]
[XmlInclude(typeof(MouseKey))]
public abstract record GenericKey
{
    [XmlElement("Code")]
    public int KeyCode { get; set; }

    protected GenericKey(int keyCode)
    {
        KeyCode = keyCode;
    }

    protected GenericKey()
    {
    }
}
