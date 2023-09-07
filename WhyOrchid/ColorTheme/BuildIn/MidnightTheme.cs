using WhyOrchid.ColorTheme.Models;

namespace WhyOrchid.ColorTheme.BuildIn;

public sealed class MidnightTheme : ColorThemeBase
{
    public override string? Name                 { get; set; } = "Midnight";
    public override string? Version              { get; set; } = "1.0";
    public override string? Creator              { get; set; } = "E4rth";
    public override string? Description          { get; set; } = "Dark Theme";
    public override string? Url                  { get; set; } = "https://github.com/E4rth";
    public override ColorSchemeBase? ColorScheme { get; set; } = new MidnightScheme();
}
