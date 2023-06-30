using System.Windows;
using System.Windows.Controls;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Controls;

public class PaletteButton : Button
{
    static PaletteButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PaletteButton), new FrameworkPropertyMetadata(typeof(PaletteButton)));
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(PaletteButton));
}
