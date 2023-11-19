using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;

[Serializable]
[XmlType("Point")]
public class InteractionPoint
{
    public string Id { get; set; } = string.Empty;

    [XmlArrayItem(ElementName = "ListOfKeys")]
    public List<List<GenericKey>> ActivationKeys { get; } = new();

    public InteractionPoint(string id, List<List<GenericKey>> activationKeys)
    {
        Id = id;
        ActivationKeys = activationKeys;
    }

    public InteractionPoint()
    {
    }
}
