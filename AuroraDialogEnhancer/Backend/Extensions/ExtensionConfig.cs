using System;
using AuroraDialogEnhancer.Backend.Hooks.Game;

namespace AuroraDialogEnhancer.Backend.Extensions;

[Serializable]
public class ExtensionConfig
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = "Unknown";

    public string DisplayName { get; set; } = "Unknown";

    public string GameLocation { get; set; } = string.Empty;

    public string LauncherLocation { get; set; } = string.Empty;

    public string ScreenshotsLocation { get; set; } = string.Empty;

    public EHookLaunchType HookLaunchType { get; set; } = EHookLaunchType.Nothing;

    public string GameProcessName { get; set; } = string.Empty;

    public string LauncherProcessName { get; set; } = string.Empty;

    public bool IsExitWithTheGame { get; set; } = false;
}
