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
        _isDialogModeFunc     = IsDialogModeBase;
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

    private bool IsDialogModeBase()
    {
        return DialogOptionFinder.IsDialogMode(_screenCaptureService.CaptureRelative(IndicationAreas[0]));
    }

    private bool IsDialogModeMultiple()
    {
        return DialogOptionFinder.IsDialogMode(IndicationAreas.Select(rectangle => _screenCaptureService.CaptureRelative(rectangle)).ToArray());
    }

    public List<Rectangle> GetDialogOptions()
    {
        return DialogOptionFinder.GetDialogOptions(_screenCaptureService.CaptureRelative(DialogOptionsArea));
    }

    public void Dispose()
    {
        DialogOptionFinder = new DialogOptionFinderEmpty();
        IndicationAreas    = Array.Empty<Rectangle>();
        DialogOptionsArea  = Rectangle.Empty;
    }
}
