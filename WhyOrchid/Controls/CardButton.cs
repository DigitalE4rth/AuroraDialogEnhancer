using System.Windows;
using System.Windows.Media;
using WhyOrchid.Controls.Config;

namespace WhyOrchid.Controls;

public class CardButton : System.Windows.Controls.Button
{
    public object LeftIcon
    {
        get => GetValue(LeftIconProperty);
        set => SetValue(LeftIconProperty, value);
    }

    public static readonly DependencyProperty LeftIconProperty = DependencyProperty.Register(
        "LeftIcon",
        typeof(object),
        typeof(CardButton));

    public Thickness LeftIconMargin
    {
        get => (Thickness)GetValue(LeftIconMarginProperty);
        set => SetValue(LeftIconMarginProperty, value);
    }

    public static readonly DependencyProperty LeftIconMarginProperty = DependencyProperty.Register(
        "LeftIconMargin",
        typeof(Thickness),
        typeof(CardButton));

    public object Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title",
        typeof(object),
        typeof(CardButton));

    public object Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        "Description",
        typeof(object),
        typeof(CardButton));

    public Thickness DescriptionMargin
    {
        get => (Thickness)GetValue(DescriptionMarginProperty);
        set => SetValue(DescriptionMarginProperty, value);
    }

    public static readonly DependencyProperty DescriptionMarginProperty = DependencyProperty.Register(
        "DescriptionMargin",
        typeof(Thickness),
        typeof(CardButton));

    public Thickness TitleDescriptionMargin
    {
        get => (Thickness)GetValue(TitleDescriptionMarginProperty);
        set => SetValue(TitleDescriptionMarginProperty, value);
    }

    public static readonly DependencyProperty TitleDescriptionMarginProperty = DependencyProperty.Register(
        "TitleDescriptionMargin",
        typeof(Thickness),
        typeof(CardButton));

    public ECardButtonContentForeground ContentForeground
    {
        get => (ECardButtonContentForeground)GetValue(ContentForegroundProperty);
        set => SetValue(ContentForegroundProperty, value);
    }

    public static readonly DependencyProperty ContentForegroundProperty = DependencyProperty.Register(
        "ContentForeground",
        typeof(ECardButtonContentForeground),
        typeof(CardButton),
        new PropertyMetadata(ECardButtonContentForeground.Primary));

    public Thickness ContentMargin
    {
        get => (Thickness)GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }

    public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register(
        "ContentMargin",
        typeof(Thickness),
        typeof(CardButton));

    public object RightIcon
    {
        get => GetValue(RightIconProperty);
        set => SetValue(RightIconProperty, value);
    }

    public static readonly DependencyProperty RightIconProperty = DependencyProperty.Register(
        "RightIcon",
        typeof(object),
        typeof(CardButton));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(CardButton));
}
