namespace AuroraDialogEnhancerExtensions.Location;

internal class LocationProviderEmpty : ILocationProvider
{
    public string LauncherLocation => string.Empty;
    public string GameLocation => string.Empty;
    public string ScreenshotsLocation => string.Empty;
}
