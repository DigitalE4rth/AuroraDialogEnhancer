using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancerExtensions.Proxy;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class ComputerVisionService : IDisposable
{
    private readonly ScreenCaptureService _screenCaptureService;

    private IDialogOptionFinder DialogOptionFinder { get; set; }
    private Rectangle[]         IndicationAreas    { get; set; }
    private Rectangle           DialogOptionsArea  { get; set; }
    private Func<bool>          _isDialogModeFunc;

    public ComputerVisionService(ScreenCaptureService screenCaptureService)
    {
        _screenCaptureService = screenCaptureService;
        DialogOptionFinder    = new DialogOptionFinderEmpty();
        IndicationAreas       = Array.Empty<Rectangle>();
        _isDialogModeFunc     = IsDialogModeSingle;
    }

    public void Initialize(DialogOptionFinderProvider provider)
    {
        DialogOptionFinder = provider.DialogOptionsFinder;
        DialogOptionsArea  = provider.Data.DialogDetectionConfig.DialogOptionsArea;
        IndicationAreas    = provider.Data.DialogDetectionConfig.IndicationAreas;

        if (provider.Data.DialogDetectionConfig.IndicationAreas.Length <= 1) return;
        _isDialogModeFunc = IsDialogModeMultiple;
    }

    public bool IsDialogMode() => _isDialogModeFunc();

    private bool IsDialogModeSingle()
    {
        using var image = _screenCaptureService.CaptureRelative(IndicationAreas[0]);
        var result = DialogOptionFinder.IsDialogMode(image);
        return result;
    }

    private bool IsDialogModeMultiple()
    {
        var images = IndicationAreas.Select(rectangle => _screenCaptureService.CaptureRelative(rectangle)).ToArray();
        var result = DialogOptionFinder.IsDialogMode(images);
        foreach (var image in images)
        {
            image.Dispose();
        }
        return result;
    }

    public List<Rectangle> GetDialogOptions()
    {
        using var image = _screenCaptureService.CaptureRelative(DialogOptionsArea);
        var result = DialogOptionFinder.GetDialogOptions(image);
        return result;
    }

    public void Dispose()
    {
        DialogOptionFinder = new DialogOptionFinderEmpty();
        IndicationAreas    = Array.Empty<Rectangle>();
        DialogOptionsArea  = Rectangle.Empty;
    }
}
