using System;
using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancerExtensions.Proxy;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class ComputerVisionService : IDisposable
{
    private readonly ScreenCaptureService _screenCaptureService;

    private IDialogOptionFinder DialogOptionFinder { get; set; }
    private Rectangle           SpeakerNameArea    { get; set; }
    private Rectangle           DialogOptionsArea  { get; set; }

    public ComputerVisionService(ScreenCaptureService screenCaptureService)
    {
        _screenCaptureService = screenCaptureService;
        DialogOptionFinder    = new DialogOptionFinderEmpty();
    }

    public void Initialize(DialogOptionFinderProvider provider)
    {
        DialogOptionFinder = provider.DialogOptionsFinder;
        SpeakerNameArea    = provider.Data.SpeakerNameArea;
        DialogOptionsArea  = provider.Data.DialogOptionsArea;
    }

    public bool IsDialogMode()
    {
        return DialogOptionFinder.IsDialogMode(_screenCaptureService.CaptureRelative(SpeakerNameArea));
    }

    public List<Rectangle> GetDialogOptions()
    {
        return DialogOptionFinder.GetDialogOptions(_screenCaptureService.CaptureRelative(DialogOptionsArea));
    }

    public void Dispose()
    {
        DialogOptionFinder = new DialogOptionFinderEmpty();
        SpeakerNameArea    = Rectangle.Empty;
        DialogOptionsArea  = Rectangle.Empty;
    }
}
