using System;
using System.IO;
using AuroraDialogEnhancerExtensions.Location;

namespace Extension.GenshinImpact.Location;

public class LocationProvider : ILocationProvider
{
    public string LauncherPath          { get; } = string.Empty;
    public string GamePath              { get; } = string.Empty;
    public string ScreenshotsFolderPath { get; } = string.Empty;

    public LocationProvider()
    {
        string installationPath;
        try
        {
            installationPath = Microsoft.Win32.Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Genshin Impact",
                "InstallPath",
                string.Empty).ToString();
        }
        catch (Exception)
        {
            return;
        }

        if (string.IsNullOrEmpty(installationPath)) return;
            
        var launcherPath = Path.Combine(installationPath, "launcher.exe");
        if (File.Exists(launcherPath))
        {
            LauncherPath = launcherPath;
        }

        const string subFolder = "Genshin Impact game";

        var gameExePath = Path.Combine(installationPath, subFolder, "GenshinImpact.exe");
        if (File.Exists(gameExePath))
        {
            GamePath = gameExePath;
        }

        var screenshotsFolderPath = Path.Combine(installationPath, subFolder, "ScreenShot");
        if (Directory.Exists(screenshotsFolderPath))
        {
            ScreenshotsFolderPath = screenshotsFolderPath;
        }
    }
}
