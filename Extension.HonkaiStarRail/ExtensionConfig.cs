using AuroraDialogEnhancerExtensions.Content;

namespace Extension.HonkaiStarRail;

public sealed class ExtensionConfig : ExtensionConfigDto
{
    public override string GameProcessName { get; protected set; } = "HonkaiStarRail";

    public override string LauncherProcessName { get; protected set; } = "launcher";
}
