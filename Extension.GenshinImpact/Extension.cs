using System;
using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.Content;
using AuroraDialogEnhancerExtensions.KeyBinding;
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

    public override ExtensionConfigDto GetExtensionConfig() => new ExtensionConfig();

    public override Bitmap GetCover() => Properties.Resources.Cover;

    public override KeyBindingProfileDto GetKeyBindingProfile() => new KeyBindingProfile();

    public override Dictionary<Size, Type> Presets { get; protected set; } = new()
    {
        { new Size(1680,1050), typeof(Preset1680X1050) },
        { new Size(1768,992),  typeof(Preset1768X992)  },
        { new Size(1920,1080), typeof(Preset1920X1080) },
        { new Size(1920,1200), typeof(Preset1920X1200) },
        { new Size(1920,1440), typeof(Preset1920X1440) },
        { new Size(2560,1440), typeof(Preset2560X1440) }
    };
}
