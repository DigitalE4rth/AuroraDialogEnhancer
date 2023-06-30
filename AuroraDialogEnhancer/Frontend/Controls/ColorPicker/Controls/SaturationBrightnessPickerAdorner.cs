using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Controls;

internal class SaturationBrightnessPickerAdorner : Adorner
{
    private static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        nameof(Position),
        typeof(Point),
        typeof(SaturationBrightnessPickerAdorner),
        new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly Brush FillBrush = Brushes.Transparent;
    private static readonly Pen RingPen = new(new SolidColorBrush(Color.FromRgb(240, 240, 240)), 2);
    private static readonly Pen RingBorderPen = new(new SolidColorBrush(Color.FromArgb(100, 60, 60, 60)), 1);


    internal SaturationBrightnessPickerAdorner(UIElement adornedElement) : base(adornedElement)
    {
        IsHitTestVisible = false;
    }

    internal Point Position
    {
        get => (Point)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawEllipse(FillBrush, RingPen, Position, 6.25, 6.25);
        drawingContext.DrawEllipse(FillBrush, RingBorderPen, Position, 7.25, 7.25);

        Effect = new DropShadowEffect
        {
            Color = (Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Shadow),
            Opacity = 0.1,
            BlurRadius = 10,
            ShadowDepth = 0
        };

        base.OnRender(drawingContext);
    }
}
