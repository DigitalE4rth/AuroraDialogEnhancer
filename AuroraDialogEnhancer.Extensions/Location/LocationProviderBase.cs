using System;

namespace AuroraDialogEnhancerExtensions.Location;

public abstract class LocationProviderBase
{
    public string LauncherLocation    { get; protected set; } = string.Empty;
    public string GameLocation        { get; protected set; } = string.Empty;
    public string ScreenshotsLocation { get; protected set; } = string.Empty;
    
    protected string GetInstallationPathByRegistry(string keyName, string valueName)
    {
        var installationPath = string.Empty;
        try
        {
            var value = Microsoft.Win32.Registry.GetValue(keyName, valueName, string.Empty);
            installationPath = value is null ? string.Empty : value.ToString();
        }
        catch (Exception)
        {
            return installationPath;
        }

        return installationPath;
    }
}

public sealed class LocationProviderEmpty : LocationProviderBase {}
