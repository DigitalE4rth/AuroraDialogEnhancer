namespace AuroraDialogEnhancerExtensions.Location;

public interface ILocationProvider
{
    public string LauncherLocation { get; }
    public string GameLocation { get; }
    public string ScreenshotsLocation { get; }
}
