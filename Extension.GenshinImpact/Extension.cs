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

    public override string Author { get; protected set; } = "E4rth";

    public override string Version { get; protected set; } = "1.0.0.0";

    public override string Link { get; protected set; } = "https://gitee.com/e4rth/aurora-dialog-enhancer";

    public override ExtensionConfigDto GetConfig() => new("GenshinImpact", "launcher");

    public override Bitmap GetCover() => Properties.Resources.Cover;

    public override IKeyBindingProfileProviderDto GetKeyBindingProfileProvider() => new KeyBindingProfileProvider();

    public override IPreset GetPreset() => new Preset();
}
