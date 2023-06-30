using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = System.Drawing.Point;

namespace AuroraDialogEnhancer.Backend.OpenCv;

public class OpenCvService
{
    private readonly ScreenCaptureService   _screenCaptureService;
    private readonly HookedGameDataProvider _hookedGameDataProvider;

    public OpenCvService(ScreenCaptureService   screenCaptureService,
                         HookedGameDataProvider hookedGameDataProvider)
    {
        _screenCaptureService   = screenCaptureService;
        _hookedGameDataProvider = hookedGameDataProvider;
    }

    /// <summary>
    /// Determines whether captured frame contains Speaker Name.
    /// </summary>
    /// <returns>
    ///   <see langword="true"/> if frame contains Speaker Name; otherwise, <see langword="false"/>.
    /// </returns>
    private bool IsFrameContainsSpeakerName(Bitmap speakerNamePositionCapturedRegion)
    {
        var speakerNameMat = speakerNamePositionCapturedRegion.ToMat();

        Cv2.InRange(
            speakerNameMat,
            _hookedGameDataProvider.Data!.CvPreset!.SpeakerNameColorRange!.Low,
            _hookedGameDataProvider.Data.CvPreset.SpeakerNameColorRange.High,
            speakerNameMat);

        var pixelsCount = Cv2.CountNonZero(speakerNameMat);
        speakerNameMat.Dispose();

        /*
        var matCropped = croppedImage.ToMat();
        Cv2.ImShow("Crop", matCropped);
        Cv2.ImShow("Mask", speakerNameMask);
        Cv2.WaitKey();
        matCropped.Dispose();
        */

        //ToDo: acquire actual threshold
        //Debug.WriteLine($"Is Speaker Name: {pixelsCount}");
        return pixelsCount > 0;
    }

    /// <summary>
    /// Gets the presence of speaker and dialog options coordinates by CV processing captured client frame.
    /// </summary>
    /// <returns>Dialog Options coordinates.</returns>
    public (bool, List<Point>) GetDialogOptionsCoordinates()
    {
        using (var speakerNamePositionBitmap = _screenCaptureService.CaptureRelative(_hookedGameDataProvider.Data!.CvPreset!.SpeakerNameSearchRegion))
        {
            if (speakerNamePositionBitmap is null || !IsFrameContainsSpeakerName(speakerNamePositionBitmap))
            {
                return (false, new List<Point>(0));
            }
        }

        var dialogOptionsRegionBitmap = _screenCaptureService.CaptureRelative(_hookedGameDataProvider.Data.CvPreset.DialogOptionSearchRegion);
        if (dialogOptionsRegionBitmap is null) return (false, new List<Point>(0));

        //dialogOptionsRegionBitmap.Save("Test.png");
        var dialogOptionsRegionMap = dialogOptionsRegionBitmap.ToMat();
        dialogOptionsRegionBitmap.Dispose();

        var detectedPoints = new List<(Point, double)>();
        var template = _hookedGameDataProvider.Data.CvPreset.DialogOptionTemplate!.TemplateMat!;
        var mask = _hookedGameDataProvider.Data.CvPreset.DialogOptionTemplate.MaskMat!;
        var matchResult = new Mat(dialogOptionsRegionMap.Rows - template.Rows + 1, dialogOptionsRegionMap.Cols - template.Cols + 1, MatType.CV_32FC1);
        var graySource = dialogOptionsRegionMap.CvtColor(ColorConversionCodes.BGR2GRAY);
        var threshold = _hookedGameDataProvider.Data.CvPreset.Threshold;

        Cv2.MatchTemplate(graySource, template, matchResult, TemplateMatchModes.CCoeffNormed, mask);
        Cv2.Threshold(matchResult, matchResult, threshold, 1.0, ThresholdTypes.Tozero);

        while (true)
        {
            Cv2.MinMaxLoc(matchResult, out _, out var maxValue, out _, out var maxLocation);

            if (!(maxValue >= threshold) || double.IsPositiveInfinity(maxValue)) break;

            detectedPoints.Add((new Point(maxLocation.X + _hookedGameDataProvider.Data.CvPreset.DialogOptionSearchRegion.X,
                                          maxLocation.Y + _hookedGameDataProvider.Data.CvPreset.DialogOptionSearchRegion.Y),
                                maxValue));

            Cv2.FloodFill(matchResult, maxLocation, new Scalar(0), out _, new Scalar(0.3), new Scalar(1.0));

            /*
                //Setup the rectangle to draw
                var r = new Rect(new Point(maxLocation.X, maxLocation.Y), new OpenCvSharp.Size(template.Width, template.Height));

                //Draw a rectangle of the matching area
                Cv2.Rectangle(source, r, Scalar.LimeGreen, 2);
             */
        }

        graySource.Dispose();
        matchResult.Dispose();
        dialogOptionsRegionMap.Dispose();

        detectedPoints = detectedPoints.OrderBy(tuple => tuple.Item1.Y).ToList();

        if (!detectedPoints.Any()) return (true, new List<Point>(0));
        //_logService.Log($"Points found: {detectedPoints.Count}", ELogEntryType.Debug);

        var resultList = new List<Point>();
        for (var index = 0; index < detectedPoints.Count; index++)
        {
            var (point, _) = detectedPoints[index];
            resultList.Add(point);
            //Debug.WriteLine($"Point: {point}, Similarity: {similarity}");
            //_logService.Log($"Point #{index + 1}: X: {point.X}, Y: {point.Y}, Similarity: {similarity:0.00}", ELogEntryType.Debug);
        }

        return (true, resultList);
    }
}
