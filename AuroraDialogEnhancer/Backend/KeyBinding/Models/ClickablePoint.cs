using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

[Serializable]
[XmlType("ClickablePoint")]
public class ClickablePoint
{
    public string Id { get; set; } = string.Empty;

    [XmlArrayItem(ElementName = "ListOfKeys")]
    public List<List<GenericKey>> Keys { get; } = new();


    public ClickablePoint(string id, List<List<GenericKey>> keys)
    {
        Id = id;
        Keys = keys;
    }

    public ClickablePoint()
    {
    }
}
