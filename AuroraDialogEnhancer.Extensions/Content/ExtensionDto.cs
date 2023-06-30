using System;
using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBinding;
using AuroraDialogEnhancerExtensions.Utilities;

namespace AuroraDialogEnhancerExtensions.Content;

public abstract class ExtensionDto
{
    public virtual string Id { get; protected set; } = Guid.NewGuid().ToString();

    public virtual string Name { get; protected set; } = "Base Extension";

    public virtual string DisplayName { get; protected set; } = "Base Extension";

    public virtual string Author { get; protected set; } = "Unknown";

    public virtual string Link { get; protected set; } = string.Empty;

    public virtual string Version { get; protected set; } = "1.0.0.0";

    public virtual Bitmap GetCover() => new(0,0);

    public virtual ExtensionConfigDto GetExtensionConfig() => new ExtensionConfigEmpty();

    public virtual KeyBindingProfileDto GetKeyBindingProfile() => new KeyBindingProfileEmpty();

    public virtual Dictionary<Size, Type> Presets { get; protected set; } = new(0);
}
