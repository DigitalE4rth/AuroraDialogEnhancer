using System.Windows;

namespace WhyOrchid.Controls;

public class MenuStateButton : MenuButton
{
    public static readonly DependencyProperty IconInactiveProperty = DependencyProperty.Register(
        "IconInactive",
        typeof(object),
        typeof(MenuStateButton));

    public object IconInactive
    {
        get => GetValue(IconInactiveProperty);
        set => SetValue(IconInactiveProperty, value);
    }
}
