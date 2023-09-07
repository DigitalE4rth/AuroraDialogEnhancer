namespace WhyOrchid.ColorTheme.Models;

public abstract class ColorThemeBase
{
    public virtual string? Name        { get; set; } = string.Empty;
    public virtual string? Version     { get; set; } = string.Empty;
    public virtual string? Creator     { get; set; } = string.Empty;
    public virtual string? Description { get; set; } = string.Empty;
    public virtual string? Url         { get; set; } = string.Empty;

    public virtual ColorSchemeBase? ColorScheme { get; set; } = new ColorScheme();
}
