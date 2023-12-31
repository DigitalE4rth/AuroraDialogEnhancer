﻿using System;

namespace AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

public class InteractionPointVmDto
{
    public string Id { get; }

    public string Name { get; }

    public string Description { get; }

    public string PathIcon { get; }

    public InteractionPointVmDto(string id, string name, string description, string pathIcon)
    {
        Id = id;
        Name = name;
        Description = description;
        PathIcon = pathIcon;
    }

    public InteractionPointVmDto(string id, string name, string pathIcon)
    {
        Id = id;
        Name = name;
        Description = string.Empty;
        PathIcon = pathIcon;
    }

    public InteractionPointVmDto()
    {
        Id = Guid.NewGuid().ToString();
        Name = "ERROR_KEY_HAS_NO_NAME";
        Description = string.Empty;
        PathIcon = string.Empty;
    }
}
