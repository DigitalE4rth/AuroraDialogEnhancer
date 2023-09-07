using System;

namespace WhyOrchid.ColorTheme.Models;

[Serializable]
public sealed class ColorScheme : ColorSchemeBase
{
    public ColorScheme()
    {
    }

    public ColorScheme(ColorSchemeBase? colorScheme)
    {
        Primary   = colorScheme?.Primary;
        OnPrimary = colorScheme?.OnPrimary;

        Secondary   = colorScheme?.Secondary;
        OnSecondary = colorScheme?.OnSecondary;

        Tertiary   = colorScheme?.OnTertiary;
        OnTertiary = colorScheme?.OnTertiary;

        Error   = colorScheme?.Error;
        OnError = colorScheme?.OnError;

        Background          = colorScheme?.Background;
        OnBackground        = colorScheme?.OnBackground;
        InverseBackground   = colorScheme?.InverseBackground;
        OnInverseBackground = colorScheme?.OnInverseBackground;

        Surface          = colorScheme?.Surface;
        OnSurface        = colorScheme?.OnSurface;
        InverseSurface   = colorScheme?.InverseSurface;
        OnInverseSurface = colorScheme?.OnInverseSurface;

        SurfaceVariant          = colorScheme?.SurfaceVariant;
        OnSurfaceVariant        = colorScheme?.OnSurfaceVariant;
        InverseSurfaceVariant   = colorScheme?.InverseSurfaceVariant;
        OnInverseSurfaceVariant = colorScheme?.OnInverseSurfaceVariant;

        Outline        = colorScheme?.Outline;
        OutlineVariant = colorScheme?.OutlineVariant;

        Interaction    = colorScheme?.Interaction;

        Shadow         = colorScheme?.Shadow;
    }
}
