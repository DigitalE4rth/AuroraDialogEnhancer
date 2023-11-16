using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;

[Serializable]
[XmlType("InteractionPoint")]
public class InteractionPoint
{
    public string Id { get; set; } = string.Empty;

    [XmlArrayItem(ElementName = "ListOfKeys")]
    public List<List<GenericKey>> Keys { get; } = new();


    public InteractionPoint(string id, List<List<GenericKey>> keys)
    {
        Id = id;
        Keys = keys;
    }

    public InteractionPoint()
    {
    }
}
