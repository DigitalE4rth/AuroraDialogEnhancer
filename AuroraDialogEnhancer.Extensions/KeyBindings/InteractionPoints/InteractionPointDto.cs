using System;
using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings.Keys;

namespace AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

public class InteractionPointDto
{
    public string Id { get; }

    public List<List<GenericKeyDto>> Keys { get; }

    public InteractionPointDto(string id, List<List<GenericKeyDto>> keys)
    {
        Id = id;
        Keys = keys;
    }

    public InteractionPointDto()
    {
        Id = Guid.NewGuid().ToString();
        Keys = new List<List<GenericKeyDto>>();
    }
}
