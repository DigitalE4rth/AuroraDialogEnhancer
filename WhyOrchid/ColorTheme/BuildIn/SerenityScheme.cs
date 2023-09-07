using WhyOrchid.ColorTheme.Models;

namespace WhyOrchid.ColorTheme.BuildIn;

public sealed class SerenityScheme : ColorSchemeBase
{
    public override string? Primary   { get; set; } = "#2cb065";
    public override string? OnPrimary { get; set; } = "#ffffff";

    public override string? Secondary   { get; set; } = "#ffffff";
    public override string? OnSecondary { get; set; } = "#000000";

    public override string? Tertiary   { get; set; } = "#645a72";
    public override string? OnTertiary { get; set; } = "#ffffff";

    public override string? Error   { get; set; } = "#b23535";
    public override string? OnError { get; set; } = "#ffffff";

    public override string? Background { get; set; } = "#f5f5f5";
    public override string? OnBackground { get; set; } = "#505050";
    public override string? InverseBackground { get; set; } = "#000000";
    public override string? OnInverseBackground { get; set; } = "#000000";

    public override string? Surface { get; set; } = "#fbfbfb";
    public override string? OnSurface { get; set; } = "#505050";
    public override string? InverseSurface { get; set; } = "#000000";
    public override string? OnInverseSurface { get; set; } = "#ffffff";

    public override string? SurfaceVariant { get; set; } = "#ffffff";
    public override string? OnSurfaceVariant { get; set; } = "#bf414141";
    public override string? InverseSurfaceVariant { get; set; } = "#000000";
    public override string? OnInverseSurfaceVariant { get; set; } = "#ffffff";

    public override string? Outline { get; set; } = "#dcdcdc";
    public override string? OutlineVariant { get; set; } = "#dcdcdc";

    public override string? Interaction { get; set; } = "#14383838";

    public override string? Shadow { get; set; } = "#000000";
}
