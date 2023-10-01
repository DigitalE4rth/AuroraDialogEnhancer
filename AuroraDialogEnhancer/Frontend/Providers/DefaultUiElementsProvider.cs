using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AuroraDialogEnhancer.Frontend.Controls.Cards;

namespace AuroraDialogEnhancer.Frontend.Providers;

internal class DefaultUiElementsProvider
{
    public Border GetDivider() => new()
    {
        Width             = 1,
        Margin            = new Thickness(10, 5, 10, 5),
        Height            = 20,
        VerticalAlignment = VerticalAlignment.Center,
        Background        = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_InverseSurface)),
        Opacity           = 0.1
    };

    public TextBlock GetTextBlock(string? text) => new()
    {
        Text              = text,
        TextWrapping      = TextWrapping.Wrap,
        VerticalAlignment = VerticalAlignment.Center,
        Foreground        = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_OnSurfaceVariant)),
    };

    public KeyCap GetKeyCap(object content) => new()
    {
        Content                  = content,
        MinHeight                = 22,
        VerticalContentAlignment = VerticalAlignment.Center,
        VerticalAlignment        = VerticalAlignment.Center
    };
}
