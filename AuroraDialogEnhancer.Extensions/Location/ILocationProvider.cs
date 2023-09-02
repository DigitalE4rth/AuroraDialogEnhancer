namespace AuroraDialogEnhancerExtensions.Location;

public interface ILocationProvider
{
    public string LauncherPath { get; }
    public string GamePath { get; }
    public string ScreenshotsFolderPath { get; }
}
