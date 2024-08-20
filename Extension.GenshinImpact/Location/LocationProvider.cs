using System.IO;
using AuroraDialogEnhancerExtensions.Location;

namespace Extension.GenshinImpact.Location;

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
        
        var installationFolderGame = GetInstallationPathByRegistry(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Genshin Impact", "InstallPath");
        const string subFolder = "Genshin Impact game";
        var gamePath = Path.Combine(installationFolderGame, subFolder, "GenshinImpact.exe");
        if (File.Exists(gamePath))
        {
            GameLocation = gamePath;
        }

        var screenshotsFolderPath = Path.Combine(installationFolderGame, subFolder, "ScreenShot");
        if (Directory.Exists(screenshotsFolderPath))
        {
            ScreenshotsLocation = screenshotsFolderPath;
        }
    }
}
