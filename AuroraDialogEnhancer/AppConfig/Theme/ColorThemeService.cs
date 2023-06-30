using System;
using System.IO;
using System.Xml.Serialization;
using AuroraDialogEnhancer.Frontend.Forms.Utils;
using WhyOrchid.ColorTheme.BuildIn;
using WhyOrchid.ColorTheme.Models;

namespace AuroraDialogEnhancer.AppConfig.Theme;

public class ColorThemeService
{
    private readonly Lazy<WhyOrchid.ColorTheme.ColorThemeService> _lazyLibColorThemeService;

    public ColorThemeService()
    {
        _lazyLibColorThemeService = new Lazy<WhyOrchid.ColorTheme.ColorThemeService>();
    }

    #region Management
    public bool Apply(EColorTheme newColorTheme)
    {
        var currentTheme = (EColorTheme)Properties.Settings.Default.UI_ThemeInfo_Type;
        if (currentTheme == newColorTheme) return false;

        if (newColorTheme == EColorTheme.System)
        {
            return ApplySystemTheme(currentTheme);
        }

        if (newColorTheme != EColorTheme.Custom)
        {
            return ApplyBuiltInTheme(newColorTheme, currentTheme);
        }

        var (isApplied, _) = Apply();
        return isApplied;
    }

    public (bool, ColorTheme?) Apply(string? filePath = null)
    {
        filePath ??= Properties.Settings.Default.UI_ThemeInfo_Location;

        var colorTheme = Deserialize(filePath);
        if (colorTheme is null) return (false, null);

        ApplyThemeWithCustomAccentColor(colorTheme);
        return (true, colorTheme);
    }

    private bool ApplySystemTheme(EColorTheme currentTheme)
    {
        var windowsSystemThemeType = GetSystemThemeType();
        var applicationSystemThemeType = Properties.Settings.Default.UI_ThemeInfo_IsSystemApplicationThemeLight;

        EBuiltInTheme? themeToApply = null;
        switch (currentTheme)
        {
            case EColorTheme.Light when !windowsSystemThemeType:
                themeToApply = EBuiltInTheme.Dark;
                Properties.Settings.Default.UI_ThemeInfo_IsSystemApplicationThemeLight = false;
                break;
            case EColorTheme.Dark when windowsSystemThemeType:
                themeToApply = EBuiltInTheme.Light;
                Properties.Settings.Default.UI_ThemeInfo_IsSystemApplicationThemeLight = true;
                break;
            case EColorTheme.System:
                switch (windowsSystemThemeType)
                {
                    case true when applicationSystemThemeType == false:
                        themeToApply = EBuiltInTheme.Light;
                        Properties.Settings.Default.UI_ThemeInfo_IsSystemApplicationThemeLight = true;
                        break;
                    case false when applicationSystemThemeType == true:
                        themeToApply = EBuiltInTheme.Dark;
                        Properties.Settings.Default.UI_ThemeInfo_IsSystemApplicationThemeLight = false;
                        break;
                }
                break;
            case EColorTheme.Custom:
                switch (windowsSystemThemeType)
                {
                    case true:
                        themeToApply = EBuiltInTheme.Light;
                        Properties.Settings.Default.UI_ThemeInfo_IsSystemApplicationThemeLight = true;
                        break;
                    case false:
                        themeToApply = EBuiltInTheme.Dark;
                        Properties.Settings.Default.UI_ThemeInfo_IsSystemApplicationThemeLight = false;
                        break;
                }
                break;
        }

        if (themeToApply is null) return false;

        Properties.Settings.Default.Save();
        ApplyThemeWithCustomAccentColor((EBuiltInTheme) themeToApply);
        return true;
    }

    private bool ApplyBuiltInTheme(EColorTheme newColorTheme, EColorTheme currentTheme)
    {
        var windowsSystemThemeType = GetSystemThemeType();

        switch (newColorTheme)
        {
            case EColorTheme.Light when currentTheme is EColorTheme.Light or EColorTheme.System && windowsSystemThemeType == true:
                return false;
            case EColorTheme.Light:
                ApplyThemeWithCustomAccentColor(EBuiltInTheme.Light);
                return true;
            case EColorTheme.Dark when currentTheme is EColorTheme.Dark or EColorTheme.System && windowsSystemThemeType == false:
                return false;
            case EColorTheme.Dark:
                ApplyThemeWithCustomAccentColor(EBuiltInTheme.Dark);
                return true;
            default:
                return false;
        }
    }

    public void ApplySystemThemeRelatedToWindowsThemeIfNecessary()
    {
        var currentTheme = (EColorTheme)Properties.Settings.Default.UI_ThemeInfo_Type;
        if (currentTheme != EColorTheme.System) return;
        ApplySystemTheme(currentTheme);
    }

    public void ResetAccentColor()
    {
        Properties.Settings.Default.UI_ThemeInfo_IsCustomAccentColor = false;

        var themeType = (EColorTheme) Properties.Settings.Default.UI_ThemeInfo_Type;
        if (themeType is not EColorTheme.Custom)
        {
            WhyOrchid.Properties.Settings.Default.Color_Primary = Properties.DefaultSettings.Default.UI_ThemeInfo_AccentColor;
            WhyOrchid.Properties.Settings.Default.Save();

            Properties.Settings.Default.Save();
            return;
        }

        var theme = Deserialize(Properties.Settings.Default.UI_ThemeInfo_Location);
        if (theme?.ColorScheme is null)
        {
            WhyOrchid.Properties.Settings.Default.Color_Primary = Properties.DefaultSettings.Default.UI_ThemeInfo_AccentColor;
            WhyOrchid.Properties.Settings.Default.Save();

            Properties.Settings.Default.Save();
            return;
        }

        WhyOrchid.Properties.Settings.Default.Color_Primary = theme.ColorScheme.Primary;
        WhyOrchid.Properties.Settings.Default.Save();

        Properties.Settings.Default.UI_ThemeInfo_CustomThemeAccentColor = theme.ColorScheme.Primary;
        Properties.Settings.Default.Save();
    }

    public void ApplyThemeWithCustomAccentColor(EBuiltInTheme builtInTheme)
    {
        _lazyLibColorThemeService.Value.Apply(builtInTheme);

        if (!Properties.Settings.Default.UI_ThemeInfo_IsCustomAccentColor)
        {
            Properties.Settings.Default.UI_ThemeInfo_CustomThemeAccentColor = Properties.DefaultSettings.Default.UI_ThemeInfo_AccentColor;
            Properties.Settings.Default.Save();
            return;
        }
        WhyOrchid.Properties.Settings.Default.Color_Primary = Properties.Settings.Default.UI_ThemeInfo_CustomThemeAccentColor;
        WhyOrchid.Properties.Settings.Default.Save();
    }

    public void ApplyThemeWithCustomAccentColor(ColorThemeBase colorTheme)
    {
        _lazyLibColorThemeService.Value.Apply(colorTheme);

        if (!Properties.Settings.Default.UI_ThemeInfo_IsCustomAccentColor) return;

        WhyOrchid.Properties.Settings.Default.Color_Primary = Properties.Settings.Default.UI_ThemeInfo_CustomThemeAccentColor;
        WhyOrchid.Properties.Settings.Default.Save();
    }
    #endregion


    #region Files
    public void Serialize(string filePath, ColorThemeBase colorTheme)
    {
        try
        {
            var serializer = XmlSerializer.FromTypes(new[] { typeof(ColorTheme) })[0];
            using var fileStream = new FileStream(filePath, FileMode.Create);
            serializer.Serialize(fileStream, new ColorTheme(colorTheme));
        }
        catch (Exception e)
        {
            new InfoDialogBuilder()
                .SetWindowTitle(Properties.Localization.Resources.Appearance_Error_Import)
                .SetMessage(e.Message + Environment.NewLine + e.InnerException?.Message)
                .SetTypeError()
                .ShowDialog();
        }
    }

    private ColorTheme? Deserialize(string filePath)
    {
        try
        {
            var serializer = XmlSerializer.FromTypes(new[] { typeof(ColorTheme) })[0];
            using var fileStream = new FileStream(filePath, FileMode.Open);
            var colorTheme = (ColorTheme)serializer.Deserialize(fileStream);

            return colorTheme;
        }
        catch (Exception e)
        {
            new InfoDialogBuilder()
                .SetWindowTitle(Properties.Localization.Resources.Appearance_Error_Export)
                .SetMessage(e.Message + Environment.NewLine + e.InnerException?.Message)
                .SetTypeError()
                .ShowDialog();

            return null;
        }
    }
    #endregion


    #region Utils
    private bool GetSystemThemeType()
    {
        try
        {
            var themeTypeValue = Microsoft.Win32.Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                "AppsUseLightTheme",
                1);

            return themeTypeValue is null || (int) themeTypeValue != 0;
        }
        catch (Exception)
        {
            // The old version of Windows does not have this value
        }

        return true;
    }

    public (string, ColorThemeBase) GetAppliedTheme()
    {
        return (EColorTheme) Properties.Settings.Default.UI_ThemeInfo_Type switch
        {
            EColorTheme.Dark   => ("Dark", new MidnightTheme()),
            EColorTheme.Light  => ("Light", new SerenityTheme()),
            EColorTheme.System => Properties.Settings.Default.UI_ThemeInfo_IsSystemApplicationThemeLight
                ? ("Light", new SerenityTheme())
                : ("Dark", new MidnightTheme()),
            _ => ("ADE_Theme", new ColorTheme())
        };
    }
    #endregion
}
