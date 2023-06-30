using System.Windows;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Controls.Cards;

internal class CardButtonWithCustomBackground : CardButton
{
    public object CustomBackground
    {
        get => GetValue(CustomBackgroundProperty);
        set => SetValue(CustomBackgroundProperty, value);
    }

    public static readonly DependencyProperty CustomBackgroundProperty = DependencyProperty.Register(
        "CustomBackground",
        typeof(object),
        typeof(CardButtonWithCustomBackground));
}
