using System.Windows;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Controls;

internal partial class TransparencyPicker : SliderPicker
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(TransparencyPicker),
        new FrameworkPropertyMetadata(Colors.Red));

    public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register(
        nameof(Alpha),
        typeof(byte),
        typeof(TransparencyPicker),
        new PropertyMetadata((byte)0, OnAlphaChanged));

    public TransparencyPicker()
    {
        InitializeComponent();
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public byte Alpha
    {
        get => (byte)GetValue(AlphaProperty);
        set => SetValue(AlphaProperty, value);
    }

    private static void OnAlphaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var picker = (TransparencyPicker)d;
        picker.AdornerVerticalPercent = (byte)e.NewValue / (double)255;
    }

    protected override void OnAdornerPositionChanged(double verticalPercent)
    {
        Alpha = (byte)(verticalPercent * 255);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        AdjustRenderPosition();
    }
}
