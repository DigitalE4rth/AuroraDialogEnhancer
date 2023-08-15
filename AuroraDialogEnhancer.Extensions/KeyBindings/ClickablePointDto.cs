﻿using System;
using System.Collections.Generic;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

public class ClickablePointDto
{
    public string Id { get; }

    public List<List<GenericKeyDto>> Keys { get; }

    public ClickablePointDto(string id, List<List<GenericKeyDto>> keys)
    {
        Id = id;
        Keys = keys;
    }

    public ClickablePointDto()
    {
        Id = Guid.NewGuid().ToString();
        Keys = new List<List<GenericKeyDto>>();
    }
}
