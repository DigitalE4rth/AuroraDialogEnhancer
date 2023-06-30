using System;
using System.IO;

namespace AuroraDialogEnhancer.AppConfig.Statics;

internal class Locations
{
    public readonly string AssemblyExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

    public readonly string AssemblyFolder = AppContext.BaseDirectory;

    public string ExtensionsFolder => Path.Combine(AssemblyFolder, "Extensions");

    public string ConfigFolder => Path.Combine(AssemblyFolder, "Config");

    public string ExtensionConfigFileName => "ExtensionConfig.xml";

    public string KeyBindingProfilesFileName => "KeyBindingProfile.xml";

    public string ThemesFolder => Path.Combine(AssemblyFolder, "Themes");

    public string ScreenshotsFolderName => "Screenshots";
}
