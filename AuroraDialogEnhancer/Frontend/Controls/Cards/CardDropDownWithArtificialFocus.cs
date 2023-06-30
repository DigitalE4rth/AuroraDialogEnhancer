﻿using System.Windows;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Controls.Cards;

public class CardDropDownWithArtificialFocus : CardDropDown
{
    public bool IsArtificiallyFocused
    {
        get => (bool)GetValue(IsArtificiallyFocusedProperty);
        set => SetValue(IsArtificiallyFocusedProperty, value);
    }

    public static readonly DependencyProperty IsArtificiallyFocusedProperty = DependencyProperty.Register(
        "IsArtificiallyFocused",
        typeof(bool),
        typeof(CardDropDownWithArtificialFocus));
}
