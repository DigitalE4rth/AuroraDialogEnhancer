using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WhyOrchid.Controls.Config;

namespace WhyOrchid.Controls;

public class CardComboBox : System.Windows.Controls.ComboBox
{
    public CardComboBox()
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

    public object LeftIcon
    {
        get => GetValue(LeftIconProperty);
        set => SetValue(LeftIconProperty, value);
    }

    public static readonly DependencyProperty LeftIconProperty = DependencyProperty.Register(
        "LeftIcon",
        typeof(object),
        typeof(CardComboBox));

    public Thickness LeftIconMargin
    {
        get => (Thickness)GetValue(LeftIconMarginProperty);
        set => SetValue(LeftIconMarginProperty, value);
    }

    public static readonly DependencyProperty LeftIconMarginProperty = DependencyProperty.Register(
        "LeftIconMargin",
        typeof(Thickness),
        typeof(CardComboBox));

    public object Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title",
        typeof(object),
        typeof(CardComboBox));

    public object Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        "Description",
        typeof(object),
        typeof(CardComboBox));

    public Thickness DescriptionMargin
    {
        get => (Thickness)GetValue(DescriptionMarginProperty);
        set => SetValue(DescriptionMarginProperty, value);
    }

    public static readonly DependencyProperty DescriptionMarginProperty = DependencyProperty.Register(
        "DescriptionMargin",
        typeof(Thickness),
        typeof(CardComboBox));

    public Thickness TitleDescriptionMargin
    {
        get => (Thickness)GetValue(TitleDescriptionMarginProperty);
        set => SetValue(TitleDescriptionMarginProperty, value);
    }

    public static readonly DependencyProperty TitleDescriptionMarginProperty = DependencyProperty.Register(
        "TitleDescriptionMargin",
        typeof(Thickness),
        typeof(CardComboBox));

    public ECardButtonContentForeground ContentForeground
    {
        get => (ECardButtonContentForeground)GetValue(ContentForegroundProperty);
        set => SetValue(ContentForegroundProperty, value);
    }

    public static readonly DependencyProperty ContentForegroundProperty = DependencyProperty.Register(
        "ContentForeground",
        typeof(ECardButtonContentForeground),
        typeof(CardComboBox),
        new PropertyMetadata(ECardButtonContentForeground.Primary));

    public Thickness ContentMargin
    {
        get => (Thickness)GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }

    public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register(
        "ContentMargin",
        typeof(Thickness),
        typeof(CardComboBox));

    public object RightIcon
    {
        get => GetValue(RightIconProperty);
        set => SetValue(RightIconProperty, value);
    }

    public static readonly DependencyProperty RightIconProperty = DependencyProperty.Register(
        "RightIcon",
        typeof(object),
        typeof(CardComboBox));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(CardComboBox));

    public SolidColorBrush PopUpBackground
    {
        get => (SolidColorBrush)GetValue(PopUpBackgroundProperty);
        set => SetValue(PopUpBackgroundProperty, value);
    }

    public static readonly DependencyProperty PopUpBackgroundProperty = DependencyProperty.Register(
        "PopUpBackground",
        typeof(SolidColorBrush),
        typeof(CardComboBox));

    public SolidColorBrush PopUpBorderBrush
    {
        get => (SolidColorBrush)GetValue(PopUpBorderBrushProperty);
        set => SetValue(PopUpBorderBrushProperty, value);
    }

    public static readonly DependencyProperty PopUpBorderBrushProperty = DependencyProperty.Register(
        "PopUpBorderBrush",
        typeof(SolidColorBrush),
        typeof(CardComboBox));

    public PlacementMode PopUpPlacementMode
    {
        get => (PlacementMode)GetValue(PopUpPlacementModeProperty);
        set => SetValue(PopUpPlacementModeProperty, value);
    }

    public static readonly DependencyProperty PopUpPlacementModeProperty = DependencyProperty.Register(
        "PopUpPlacementMode",
        typeof(PlacementMode),
        typeof(CardComboBox));

    public double PopupMinimumWidth
    {
        get => (double)GetValue(PopupMinimumWidthProperty);
        set => SetValue(PopupMinimumWidthProperty, value);
    }

    public static readonly DependencyProperty PopupMinimumWidthProperty = DependencyProperty.Register(
        "PopupMinimumWidth",
        typeof(double),
        typeof(CardComboBox));

    public double PopupVerticalOffset
    {
        get => (double)GetValue(PopupVerticalOffsetProperty);
        set => SetValue(PopupVerticalOffsetProperty, value);
    }

    public static readonly DependencyProperty PopupVerticalOffsetProperty = DependencyProperty.Register(
        "PopupVerticalOffset",
        typeof(double),
        typeof(CardComboBox));

    private void CardComboBox_Unloaded(object sender, RoutedEventArgs e)
    {
        PreviewMouseWheel -= CardComboBox_PreviewMouseWheel;
        Unloaded          -= CardComboBox_Unloaded;
    }
}
