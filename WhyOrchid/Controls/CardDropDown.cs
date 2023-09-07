using System.Windows;

namespace WhyOrchid.Controls;

public class CardDropDown : CardComboBox
{
    public object BodyContent
    {
        get => GetValue(BodyContentProperty);
        set => SetValue(BodyContentProperty, value);
    }

    public static readonly DependencyProperty BodyContentProperty = DependencyProperty.Register(
        "BodyContent",
        typeof(object),
        typeof(CardDropDown));

    public Thickness ContentPadding
    {
        get => (Thickness)GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    public static readonly DependencyProperty ContentPaddingProperty = DependencyProperty.Register(
        "ContentPadding",
        typeof(Thickness),
        typeof(CardDropDown));
}
