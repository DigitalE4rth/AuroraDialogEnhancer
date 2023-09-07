using WhyOrchid.ColorTheme.BuildIn;
using WhyOrchid.ColorTheme.Models;

namespace WhyOrchid.ColorTheme;

public class ColorThemeService
{
    public bool Apply(EBuiltInTheme builtInTheme) => builtInTheme == EBuiltInTheme.Light ? Apply(new SerenityTheme()) : Apply(new MidnightTheme());

    public bool Apply(ColorThemeBase colorTheme)
    {
        if (colorTheme.ColorScheme is null) return false;
        
        Properties.Settings.Default.Color_Primary   = colorTheme.ColorScheme.Primary;
        Properties.Settings.Default.Color_OnPrimary = colorTheme.ColorScheme.OnPrimary;

        Properties.Settings.Default.Color_Secondary   = colorTheme.ColorScheme.Secondary;
        Properties.Settings.Default.Color_OnSecondary = colorTheme.ColorScheme.OnSecondary;

        Properties.Settings.Default.Color_Tertiary   = colorTheme.ColorScheme.Tertiary;
        Properties.Settings.Default.Color_OnTertiary = colorTheme.ColorScheme.OnTertiary;

        Properties.Settings.Default.Color_Error   = colorTheme.ColorScheme.Error;
        Properties.Settings.Default.Color_OnError = colorTheme.ColorScheme.OnError;

        Properties.Settings.Default.Color_Background          = colorTheme.ColorScheme.Background;
        Properties.Settings.Default.Color_OnBackground        = colorTheme.ColorScheme.OnBackground;
        Properties.Settings.Default.Color_InverseBackground   = colorTheme.ColorScheme.InverseBackground;
        Properties.Settings.Default.Color_OnInverseBackground = colorTheme.ColorScheme.OnInverseBackground;

        Properties.Settings.Default.Color_Surface          = colorTheme.ColorScheme.Surface;
        Properties.Settings.Default.Color_OnSurface        = colorTheme.ColorScheme.OnSurface;
        Properties.Settings.Default.Color_InverseSurface   = colorTheme.ColorScheme.InverseSurface;
        Properties.Settings.Default.Color_OnInverseSurface = colorTheme.ColorScheme.OnInverseSurface;

        Properties.Settings.Default.Color_SurfaceVariant          = colorTheme.ColorScheme.SurfaceVariant;
        Properties.Settings.Default.Color_OnSurfaceVariant        = colorTheme.ColorScheme.OnSurfaceVariant;
        Properties.Settings.Default.Color_InverseSurfaceVariant   = colorTheme.ColorScheme.InverseSurfaceVariant;
        Properties.Settings.Default.Color_OnInverseSurfaceVariant = colorTheme.ColorScheme.OnInverseSurfaceVariant;

        Properties.Settings.Default.Color_Outline        = colorTheme.ColorScheme.Outline;
        Properties.Settings.Default.Color_OutlineVariant = colorTheme.ColorScheme.OutlineVariant;

        Properties.Settings.Default.Color_Interaction = colorTheme.ColorScheme.Interaction;

        Properties.Settings.Default.Color_Shadow = colorTheme.ColorScheme.Shadow;

        Properties.Settings.Default.Save();
        return true;
    }
}
