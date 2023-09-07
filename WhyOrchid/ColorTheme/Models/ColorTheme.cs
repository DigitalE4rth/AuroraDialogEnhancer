using System;
using System.Xml.Serialization;

namespace WhyOrchid.ColorTheme.Models;

[Serializable]
public sealed class ColorTheme : ColorThemeBase
{
    public ColorTheme()
    {
    }

    public ColorTheme(ColorThemeBase colorTheme)
    {
        Name        = colorTheme.Name;
        Version     = colorTheme.Version;
        Creator     = colorTheme.Creator;
        Description = colorTheme.Description;
        Url         = colorTheme.Url;
        ColorScheme = new ColorScheme(colorTheme.ColorScheme);
    }
}
