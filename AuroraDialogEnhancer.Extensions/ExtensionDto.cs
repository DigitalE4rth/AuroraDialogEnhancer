using System;
using System.Drawing;
using AuroraDialogEnhancerExtensions.Content;
using AuroraDialogEnhancerExtensions.KeyBindings.Models;
using AuroraDialogEnhancerExtensions.Location;
using AuroraDialogEnhancerExtensions.Proxy;

namespace AuroraDialogEnhancerExtensions;

public abstract class ExtensionDto
{
    public virtual string Id { get; protected set; } = Guid.NewGuid().ToString();

    public virtual string Name { get; protected set; } = "Base Extension";

    public virtual string DisplayName { get; protected set; } = "Base Extension";

    public virtual string Author { get; protected set; } = "Unknown";

    public virtual string Link { get; protected set; } = string.Empty;

    public virtual string Version { get; protected set; } = "1.0.0.0";

    public virtual Bitmap GetCover() => new(0,0);

    public virtual ExtensionConfigDto GetConfig() => new();

    public virtual ILocationProvider GetLocationProvider() => new LocationProviderEmpty();

    public virtual IKeyBindingProfileProviderDto GetKeyBindingProfileProvider() => new KeyBindingProfileProviderEmpty();

    public virtual PresetBase GetPreset() => new PresetEmpty();
}
