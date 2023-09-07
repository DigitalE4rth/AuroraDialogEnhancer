using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WhyOrchid.Controls;

public class ComboBox : System.Windows.Controls.ComboBox
{
    public ComboBox()
    {
        Unloaded += CardComboBox_Unloaded;
        PreviewMouseWheel += CardComboBox_PreviewMouseWheel;
    }

    private void CardComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (IsDropDownOpen) return;

        e.Handled = true;
        ((FrameworkElement) Parent).RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = MouseWheelEvent,
            Source = sender
        });
    }

    public object Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title",
        typeof(object),
        typeof(ComboBox));

    public bool IsMenu
    {
        get => (bool)GetValue(IsMenuProperty);
        set => SetValue(IsMenuProperty, value);
    }

    public static readonly DependencyProperty IsMenuProperty = DependencyProperty.Register(
        "IsMenu",
        typeof(object),
        typeof(ComboBox));

    public bool IsFullWidthPopUp
    {
        get => (bool)GetValue(IsFullWidthPopUpProperty);
        set => SetValue(IsFullWidthPopUpProperty, value);
    }

    public static readonly DependencyProperty IsFullWidthPopUpProperty = DependencyProperty.Register(
        "IsFullWidthPopUp",
        typeof(object),
        typeof(ComboBox));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(ComboBox));

    public SolidColorBrush PopUpBackground
    {
        get => (SolidColorBrush)GetValue(PopUpBackgroundProperty);
        set => SetValue(PopUpBackgroundProperty, value);
    }

    public static readonly DependencyProperty PopUpBackgroundProperty = DependencyProperty.Register(
        "PopUpBackground",
        typeof(SolidColorBrush),
        typeof(ComboBox));

    public SolidColorBrush PopUpBorderBrush
    {
        get => (SolidColorBrush)GetValue(PopUpBorderBrushProperty);
        set => SetValue(PopUpBorderBrushProperty, value);
    }

    public static readonly DependencyProperty PopUpBorderBrushProperty = DependencyProperty.Register(
        "PopUpBorderBrush",
        typeof(SolidColorBrush),
        typeof(ComboBox));

    public PlacementMode PopUpPlacementMode
    {
        get => (PlacementMode)GetValue(PopUpPlacementModeProperty);
        set => SetValue(PopUpPlacementModeProperty, value);
    }

    public static readonly DependencyProperty PopUpPlacementModeProperty = DependencyProperty.Register(
        "PopUpPlacementMode",
        typeof(PlacementMode),
        typeof(ComboBox));

    public double PopupMinimumWidth
    {
        get => (double)GetValue(PopupMinimumWidthProperty);
        set => SetValue(PopupMinimumWidthProperty, value);
    }

    public static readonly DependencyProperty PopupMinimumWidthProperty = DependencyProperty.Register(
        "PopupMinimumWidth",
        typeof(double),
        typeof(ComboBox));

    public double PopupVerticalOffset
    {
        get => (double)GetValue(PopupVerticalOffsetProperty);
        set => SetValue(PopupVerticalOffsetProperty, value);
    }

    public static readonly DependencyProperty PopupVerticalOffsetProperty = DependencyProperty.Register(
        "PopupVerticalOffset",
        typeof(double),
        typeof(ComboBox));

    private void CardComboBox_Unloaded(object sender, RoutedEventArgs e)
    {
        PreviewMouseWheel -= CardComboBox_PreviewMouseWheel;
        Unloaded -= CardComboBox_Unloaded;
    }
}
