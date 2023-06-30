namespace AuroraDialogEnhancerExtensions.Content;

public abstract class ExtensionConfigDto
{
    public virtual string GameProcessName { get; protected set; } = string.Empty;

    public virtual string LauncherProcessName { get; protected set; } = string.Empty;
}
