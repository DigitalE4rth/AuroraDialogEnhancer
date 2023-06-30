using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Services;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Controls;

internal abstract class SliderPicker : UserControl
{
    private readonly PointClamperService _pointClamperService;
    private readonly SliderPickerAdorner _adorner;

    protected SliderPicker()
    {
        _pointClamperService = new PointClamperService();
        _adorner = new SliderPickerAdorner(this);
        Unloaded += SliderPicker_Unloaded;
        Loaded += SliderPicker_Loaded;
    }

    protected double AdornerVerticalPercent
    {
        get => _adorner.VerticalPercent;
        set => _adorner.VerticalPercent = value;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        Mouse.Capture(this);
        UpdateAdorner(_pointClamperService.Clamp(this));
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        Mouse.Capture(null);
        UpdateAdorner(_pointClamperService.Clamp(this));
    }

    protected virtual void OnAdornerPositionChanged(double verticalPercent)
    {
    }

    private void UpdateAdorner(Point mousePos)
    {
        var verticalPercent = mousePos.Y / ActualHeight;
        _adorner.VerticalPercent = verticalPercent;
        OnAdornerPositionChanged(verticalPercent);
    }

    public void AdjustRenderPosition()
    {
        _adorner.ElementSize = new Rect(new Size(ActualWidth, ActualHeight));
    }

    private void SliderPicker_Loaded(object sender, RoutedEventArgs e)
    {
        _adorner.ElementSize = new Rect(new Size(ActualWidth, ActualHeight));
        AdornerLayer.GetAdornerLayer(this)!.Add(_adorner);
    }

    private void SliderPicker_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= SliderPicker_Unloaded;
        Loaded -= SliderPicker_Loaded;
    }
}
