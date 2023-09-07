using System.Windows;

namespace WhyOrchid.Controls;

public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton
{
    public CornerRadius CornerRadius
    {
        get => (CornerRadius) GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(ToggleButton));
}
