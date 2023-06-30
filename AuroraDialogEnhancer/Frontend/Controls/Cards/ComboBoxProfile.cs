using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.Cards;

public class ComboBoxProfile : System.Windows.Controls.ComboBox
{
    public ComboBoxProfile()
    {
        Unloaded += ComboBox_Unloaded;
        PreviewMouseWheel += ComboBox_PreviewMouseWheel;
    }

    private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;

        var maxIndex = Items.Count - 1;
        if (maxIndex == 0) return;
        
        if (e.Delta < 0)
        {
            if (SelectedIndex == maxIndex)
            {
                SelectedIndex = 0;
                return;
            }

            SelectedIndex++;
            return;
        }

        if (SelectedIndex == 0)
        {
            SelectedIndex = maxIndex;
            return;
        }

        SelectedIndex--;
    }

    public object LeftIcon
    {
        get => GetValue(LeftIconProperty);
        set => SetValue(LeftIconProperty, value);
    }

    public static readonly DependencyProperty LeftIconProperty = DependencyProperty.Register(
        "LeftIcon",
        typeof(object),
        typeof(ComboBoxProfile));

    public Thickness LeftIconMargin
    {
        get => (Thickness)GetValue(LeftIconMarginProperty);
        set => SetValue(LeftIconMarginProperty, value);
    }

    public static readonly DependencyProperty LeftIconMarginProperty = DependencyProperty.Register(
        "LeftIconMargin",
        typeof(Thickness),
        typeof(ComboBoxProfile));

    public object RightIcon
    {
        get => GetValue(RightIconProperty);
        set => SetValue(RightIconProperty, value);
    }

    public static readonly DependencyProperty RightIconProperty = DependencyProperty.Register(
        "RightIcon",
        typeof(object),
        typeof(ComboBoxProfile));

    public object CustomContent
    {
        get => GetValue(CustomContentProperty);
        set => SetValue(CustomContentProperty, value);
    }

    public static readonly DependencyProperty CustomContentProperty = DependencyProperty.Register(
        "CustomContent",
        typeof(object),
        typeof(ComboBoxProfile));

    public Thickness RightIconMargin
    {
        get => (Thickness)GetValue(RightIconMarginProperty);
        set => SetValue(RightIconMarginProperty, value);
    }

    public static readonly DependencyProperty RightIconMarginProperty = DependencyProperty.Register(
        "RightIconMargin",
        typeof(Thickness),
        typeof(ComboBoxProfile));

    public SolidColorBrush PopUpBackground
    {
        get => (SolidColorBrush)GetValue(PopUpBackgroundProperty);
        set => SetValue(PopUpBackgroundProperty, value);
    }

    public static readonly DependencyProperty PopUpBackgroundProperty = DependencyProperty.Register(
        "PopUpBackground",
        typeof(SolidColorBrush),
        typeof(ComboBoxProfile));

    public SolidColorBrush PopUpBorderBrush
    {
        get => (SolidColorBrush)GetValue(PopUpBorderBrushProperty);
        set => SetValue(PopUpBorderBrushProperty, value);
    }

    public static readonly DependencyProperty PopUpBorderBrushProperty = DependencyProperty.Register(
        "PopUpBorderBrush",
        typeof(SolidColorBrush),
        typeof(ComboBoxProfile));

    public PlacementMode PopUpPlacementMode
    {
        get => (PlacementMode)GetValue(PopUpPlacementModeProperty);
        set => SetValue(PopUpPlacementModeProperty, value);
    }

    public static readonly DependencyProperty PopUpPlacementModeProperty = DependencyProperty.Register(
        "PopUpPlacementMode",
        typeof(PlacementMode),
        typeof(ComboBoxProfile));

    public double PopupVerticalOffset
    {
        get => (double)GetValue(PopupVerticalOffsetProperty);
        set => SetValue(PopupVerticalOffsetProperty, value);
    }

    public static readonly DependencyProperty PopupVerticalOffsetProperty = DependencyProperty.Register(
        "PopupVerticalOffset",
        typeof(double),
        typeof(ComboBoxProfile));

    public double PopupMinimumWidth
    {
        get => (double)GetValue(PopupMinimumWidthProperty);
        set => SetValue(PopupMinimumWidthProperty, value);
    }

    public static readonly DependencyProperty PopupMinimumWidthProperty = DependencyProperty.Register(
        "PopupMinimumWidth",
        typeof(double),
        typeof(ComboBoxProfile));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(ComboBoxProfile));

    private void ComboBox_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= ComboBox_Unloaded;
        PreviewMouseWheel -= ComboBox_PreviewMouseWheel;
    }
}
