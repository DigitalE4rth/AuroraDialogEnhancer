using System.Windows;
using System.Windows.Media;

namespace WhyOrchid.Controls;

public class ListBox : System.Windows.Controls.ListBox
{
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(ListBox));

    public SolidColorBrush ScrollbarBackground
    {
        get => (SolidColorBrush)GetValue(ScrollbarBackgroundProperty);
        set => SetValue(ScrollbarBackgroundProperty, value);
    }

    public static readonly DependencyProperty ScrollbarBackgroundProperty = DependencyProperty.Register(
        "ScrollbarBackground",
        typeof(SolidColorBrush),
        typeof(ListBox));
}
