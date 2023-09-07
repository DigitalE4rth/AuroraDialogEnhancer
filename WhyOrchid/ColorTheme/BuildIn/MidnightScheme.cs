using WhyOrchid.ColorTheme.Models;

namespace WhyOrchid.ColorTheme.BuildIn;

public sealed class MidnightScheme : ColorSchemeBase
{
    public override string? Primary { get; set; } = "#2cb065";
    public override string? OnPrimary { get; set; } = "#f0f0f0";

    public override string? Secondary { get; set; } = "#222222";
    public override string? OnSecondary { get; set; } = "#f0f0f0";

    public override string? Tertiary { get; set; } = "#6981d0";
    public override string? OnTertiary { get; set; } = "#f0f0f0";

    public override string? Error { get; set; } = "#b23535";
    public override string? OnError { get; set; } = "#ffffff";

    public override string? Background { get; set; } = "#141414";
    public override string? OnBackground { get; set; } = "#dadada";
    public override string? InverseBackground { get; set; } = "#ffffff";
    public override string? OnInverseBackground { get; set; } = "#000000";

    public override string? Surface { get; set; } = "#222222";
    public override string? OnSurface { get; set; } = "#dadada";
    public override string? InverseSurface { get; set; } = "#ffffff";
    public override string? OnInverseSurface { get; set; } = "#000000";

    public override string? SurfaceVariant { get; set; } = "#292929";
    public override string? OnSurfaceVariant { get; set; } = "#8b8b8b";
    public override string? InverseSurfaceVariant { get; set; } = "#ffffff";
    public override string? OnInverseSurfaceVariant { get; set; } = "#000000";

    public override string? Outline { get; set; } = "#3f3f3f";
    public override string? OutlineVariant { get; set; } = "#292929";

    public override string? Interaction { get; set; } = "#0dffffff";

    public override string? Shadow { get; set; } = "#000000";
}
