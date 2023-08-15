namespace AuroraDialogEnhancerExtensions.Content;

public class ExtensionConfigDto
{
    public string GameProcessName { get; } = string.Empty;

    public string LauncherProcessName { get; } = string.Empty;

    public ExtensionConfigDto(string gameProcessName, string launcherProcessName)
    {
        GameProcessName = gameProcessName;
        LauncherProcessName = launcherProcessName;
    }

    public ExtensionConfigDto()
    {
    }
}
