using AuroraDialogEnhancerExtensions.Content;

namespace Extension.GenshinImpact;

public sealed class ExtensionConfig : ExtensionConfigDto
{
    public override string GameProcessName { get; protected set; } = "GenshinImpact";

    public override string LauncherProcessName { get; protected set; } = "launcher";
}
