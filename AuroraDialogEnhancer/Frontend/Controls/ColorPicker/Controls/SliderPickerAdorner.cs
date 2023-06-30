using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Controls;

internal class SliderPickerAdorner : Adorner
{
    private static readonly DependencyProperty VerticalPercentProperty = DependencyProperty.Register(
        nameof(VerticalPercent),
        typeof(double),
        typeof(SliderPickerAdorner),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(SliderPickerAdorner),
        new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender));

    private readonly Pen _pinButtonOutline;
    private readonly Brush _pinButtonBackground;

    public SliderPickerAdorner(UIElement adornedElement) : base(adornedElement)
    {
        IsHitTestVisible = false;
        _pinButtonBackground = new SolidColorBrush(Color.FromRgb(240,240,240));
        _pinButtonOutline = new Pen(new SolidColorBrush(Color.FromArgb(100, 60, 60, 60)), 1);
    }

    public double VerticalPercent
    {
        get => (double)GetValue(VerticalPercentProperty);
        set => SetValue(VerticalPercentProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public Rect ElementSize { get; set; }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var radius = (ElementSize.Width > ElementSize.Height ? ElementSize.Height : ElementSize.Width) / 2;
        var ellipseGeometry = new EllipseGeometry(new Point(radius, ElementSize.Height * VerticalPercent), radius + 1.25, radius + 1.25);

        drawingContext.DrawGeometry(_pinButtonBackground, _pinButtonOutline, ellipseGeometry);

        Effect = new DropShadowEffect
        {
            Color = (Color)ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Shadow),
            Opacity = 0.1,
            BlurRadius = 10,
            ShadowDepth = 0
        };

        base.OnRender(drawingContext);
    }
}
