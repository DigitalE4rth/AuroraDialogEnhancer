using System;
using System.IO;
using AuroraDialogEnhancerExtensions.Location;

namespace Extension.HonkaiStarRail.Location;

public class LocationProvider : LocationProviderBase
{
    public LocationProvider()
    {
        var installationFolderLauncher = GetInstallationPathByRegistry(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\HYP_1_0_global", "InstallPath");
        var launcherPath = Path.Combine(installationFolderLauncher, "launcher.exe");
        if (File.Exists(launcherPath))
        {
            LauncherLocation = launcherPath;
        } 
        
        var installationFolderGame = GetInstallationPathByRegistry(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Star Rail", "InstallPath");
        const string subFolder = "Games";
        var gamePath = Path.Combine(installationFolderGame, subFolder, "StarRail.exe");
        if (File.Exists(gamePath))
        {
            GameLocation = gamePath;
        }
        
        var screenshotsFolderPath = Path.Combine(installationFolderGame, subFolder, "StarRail_Data", "ScreenShots");
        if (Directory.Exists(screenshotsFolderPath))
        {
            ScreenshotsLocation = screenshotsFolderPath;
        }
    }
}
