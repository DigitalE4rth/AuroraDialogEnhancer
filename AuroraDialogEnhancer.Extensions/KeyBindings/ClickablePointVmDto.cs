using System;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

public class ClickablePointVmDto
{
    public string Id { get; }

    public string Name { get; }

    public string Description { get; }

    public string PathIcon { get; }

    public ClickablePointVmDto(string id, string name, string description, string pathIcon)
    {
        Id = id;
        Name = name;
        Description = description;
        PathIcon = pathIcon;
    }

    public ClickablePointVmDto()
    {
        Id = Guid.NewGuid().ToString();
        Name = "ERROR_KEY_HAS_NO_NAME";
        Description = string.Empty;
        PathIcon = string.Empty;
    }
}
