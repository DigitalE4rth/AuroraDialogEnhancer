using System.Drawing;
using AuroraDialogEnhancerExtensions;
using AuroraDialogEnhancerExtensions.Content;

namespace Extension.HonkaiStarRail;

public sealed class Extension : ExtensionDto
{
    public override string Id { get; protected set; } = "HSR";

    public override string Name { get; protected set; } = "Honkai Star Rail";

    public override string DisplayName { get; protected set; } = "Honkai: Star Rail";

    public override string Author { get; protected set; } = "E4rth";

    public override string Version { get; protected set; } = typeof(Extension).Assembly.GetName().Version.ToString();

    public override string Link { get; protected set; } = "https://gitee.com/e4rth/aurora-dialog-enhancer";

    public override ExtensionConfigDto GetConfig() => new("HonkaiStarRail", "launcher");

    public override Bitmap GetCover() => Properties.Resources.Cover;
}
