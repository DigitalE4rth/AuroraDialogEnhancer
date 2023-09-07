using System.Windows;
using System.Windows.Controls;

namespace WhyOrchid.Controls;

public class MenuButton : RadioButton
{
    public object Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        "Icon",
        typeof(object),
        typeof(MenuButton));

    public Thickness IconMargin
    {
        get => (Thickness)GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register(
        "IconMargin",
        typeof(Thickness),
        typeof(MenuButton));

    public Thickness ContentMargin
    {
        get => (Thickness)GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }

    public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register(
        "ContentMargin",
        typeof(Thickness),
        typeof(MenuButton));


    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(MenuButton));
}
