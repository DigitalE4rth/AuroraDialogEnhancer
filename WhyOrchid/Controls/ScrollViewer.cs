using System.Windows;
using System.Windows.Media;

namespace WhyOrchid.Controls;
public class ScrollViewer : System.Windows.Controls.ScrollViewer
{
    public Thickness VerticalScrollBorderThickness
    {
        get => (Thickness)GetValue(VerticalScrollBorderThicknessProperty);
        set => SetValue(VerticalScrollBorderThicknessProperty, value);
    }

    public static readonly DependencyProperty VerticalScrollBorderThicknessProperty = DependencyProperty.Register(
        "VerticalScrollBorderThickness",
        typeof(Thickness),
        typeof(ScrollViewer));

    public Thickness HorizontalScrollBorderThickness
    {
        get => (Thickness)GetValue(HorizontalScrollBorderThicknessProperty);
        set => SetValue(HorizontalScrollBorderThicknessProperty, value);
    }

    public static readonly DependencyProperty HorizontalScrollBorderThicknessProperty = DependencyProperty.Register(
        "HorizontalScrollBorderThickness",
        typeof(Thickness),
        typeof(ScrollViewer));

    public bool IsInnerScrollbarsArrangement
    {
        get => (bool)GetValue(IsInnerScrollbarsArrangementProperty);
        set => SetValue(IsInnerScrollbarsArrangementProperty, value);
    }

    public static readonly DependencyProperty IsInnerScrollbarsArrangementProperty = DependencyProperty.Register(
        "IsInnerScrollbarsArrangement",
        typeof(bool),
        typeof(ScrollViewer));

    public SolidColorBrush ScrollbarBackground
    {
        get => (SolidColorBrush)GetValue(ScrollbarBackgroundProperty);
        set => SetValue(ScrollbarBackgroundProperty, value);
    }

    public static readonly DependencyProperty ScrollbarBackgroundProperty = DependencyProperty.Register(
        "ScrollbarBackground",
        typeof(SolidColorBrush),
        typeof(ScrollViewer));
}
