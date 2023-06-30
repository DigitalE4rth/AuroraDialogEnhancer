using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Services;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Controls;

internal partial class SaturationBrightnessPicker : UserControl
{
    private readonly PointClamperService _pointClamperService;

    public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
        nameof(Hue),
        typeof(double),
        typeof(SaturationBrightnessPicker),
        new PropertyMetadata(0.0));

    public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(
        nameof(Saturation),
        typeof(double),
        typeof(SaturationBrightnessPicker),
        new PropertyMetadata(0.0, OnSaturationChanged));

    public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register(
        nameof(Brightness),
            typeof(double),
            typeof(SaturationBrightnessPicker),
            new PropertyMetadata(0.0, OnBrightnessChanged));

    private readonly SaturationBrightnessPickerAdorner _adorner;

    public SaturationBrightnessPicker()
    {
        _pointClamperService = new PointClamperService();
        InitializeComponent();

        _adorner = new SaturationBrightnessPickerAdorner(this);
        Unloaded += SaturationBrightnessPicker_Unloaded;
        Loaded += SaturationBrightnessPicker_OnLoaded;
    }

    public double Hue
    {
        get => (double)GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    public double Saturation
    {
        get => (double)GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, value);
    }

    public double Brightness
    {
        get => (double)GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        Mouse.Capture(this);
        var pos = _pointClamperService.Clamp(this);
        Update(pos);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        Mouse.Capture(null);
        var pos = _pointClamperService.Clamp(this);
        Update(pos);
    }

    private static void OnSaturationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var picker = (SaturationBrightnessPicker)o;
        var sat = (double)e.NewValue;
        var pos = picker._adorner.Position;
        picker._adorner.Position = new Point(sat * picker.ActualWidth, pos.Y);
    }

    private static void OnBrightnessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var picker = (SaturationBrightnessPicker)o;
        var bright = (double)e.NewValue;
        var pos = picker._adorner.Position;
        picker._adorner.Position = new Point(pos.X, (1 - bright) * picker.ActualHeight);
    }

    private void SaturationBrightnessPicker_OnLoaded(object sender, RoutedEventArgs e)
    {
        AdornerLayer.GetAdornerLayer(this)!.Add(_adorner);
        _adorner.Position = new Point(Saturation * ActualWidth, (1 - Brightness) * ActualHeight);
    }

    private void Update(Point p)
    {
        _adorner.Position = p;
        Saturation = p.X / ActualWidth;
        Brightness = 1 - (p.Y / ActualHeight); // directions reversed
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        _adorner.Position = new Point(Saturation * ActualWidth, (1 - Brightness) * ActualHeight);
    }

    private void SaturationBrightnessPicker_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= SaturationBrightnessPicker_Unloaded;
        Loaded -= SaturationBrightnessPicker_OnLoaded;
    }
}
