using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AuroraDialogEnhancer.Frontend.Controls.Cards;

[ContentProperty("Content")]
public class KeyCap : Control
{
    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        "Content",
        typeof(object),
        typeof(KeyCap),
        new PropertyMetadata(null));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(KeyCap),
        new PropertyMetadata(default));
}
