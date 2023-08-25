using System.Drawing;
using AuroraDialogEnhancerExtensions;
using AuroraDialogEnhancerExtensions.Content;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.KeyBindings;
using Extension.GenshinImpact.Presets;

namespace Extension.GenshinImpact;

public sealed class Extension : ExtensionDto
{
    public override string Id { get; protected set; } = "GI";

    public override string Name { get; protected set; } = "Genshin Impact";

    public override string DisplayName { get; protected set; } = "Genshin Impact";

    public override string Author { get; protected set; } = "DigitalE4rth";

    public override string Version { get; protected set; } = typeof(Extension).Assembly.GetName().Version.ToString();

    public override string Link { get; protected set; } = "https://github.com/DigitalE4rth/AuroraDialogEnhancer";

    public override ExtensionConfigDto GetConfig() => new("GenshinImpact", "launcher");

    public override Bitmap GetCover() => Properties.Resources.Cover;

    public override IKeyBindingProfileProviderDto GetKeyBindingProfileProvider() => new KeyBindingProfileProvider();

    public override IPreset GetPreset() => new Preset();
}
