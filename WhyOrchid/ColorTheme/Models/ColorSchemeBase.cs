using System.Xml.Serialization;

namespace WhyOrchid.ColorTheme.Models;

[XmlInclude(typeof(ColorScheme))]
public abstract class ColorSchemeBase
{
    public virtual string? Primary { get; set; } = string.Empty;
    public virtual string? OnPrimary { get; set; } = string.Empty;

    public virtual string? Secondary { get; set; } = string.Empty;
    public virtual string? OnSecondary { get; set; } = string.Empty;

    public virtual string? Tertiary { get; set; } = string.Empty;
    public virtual string? OnTertiary { get; set; } = string.Empty;

    public virtual string? Error { get; set; } = string.Empty;
    public virtual string? OnError { get; set; } = string.Empty;

    public virtual string? Background { get; set; } = string.Empty;
    public virtual string? OnBackground { get; set; } = string.Empty;
    public virtual string? InverseBackground { get; set; } = string.Empty;
    public virtual string? OnInverseBackground { get; set; } = string.Empty;

    public virtual string? Surface { get; set; } = string.Empty;
    public virtual string? OnSurface { get; set; } = string.Empty;
    public virtual string? InverseSurface { get; set; } = string.Empty;
    public virtual string? OnInverseSurface { get; set; } = string.Empty;

    public virtual string? SurfaceVariant { get; set; } = string.Empty;
    public virtual string? OnSurfaceVariant { get; set; } = string.Empty;
    public virtual string? InverseSurfaceVariant { get; set; } = string.Empty;
    public virtual string? OnInverseSurfaceVariant { get; set; } = string.Empty;

    public virtual string? Outline { get; set; } = string.Empty;
    public virtual string? OutlineVariant { get; set; } = string.Empty;

    public virtual string? Interaction { get; set; } = string.Empty;

    public virtual string? Shadow { get; set; } = string.Empty;
}
