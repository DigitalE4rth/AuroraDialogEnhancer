using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Proxy;
using AuroraDialogEnhancerExtensions.Services;
using Extension.HonkaiStarRail.Templates;
using Extension.HonkaiStarRail.Utils;

namespace Extension.HonkaiStarRail.Services;

public class DialogOptionFinderDebug : IDialogOptionFinder
{
    private readonly SearchTemplate         _searchTemplate;
    private readonly BitmapUtils            _bitmapUtils;
    private readonly DialogOptionsFinderRgb _finderRgb;
    private readonly DialogOptionsFinderHsb _finderHsb;
    private int                             _speakerNameFirstLineY;
    
    public DialogOptionFinderDebug(SearchTemplate searchTemplate)
    {
        _searchTemplate = searchTemplate;
        _bitmapUtils    = new BitmapUtils();
        _finderRgb      = new DialogOptionsFinderRgb(searchTemplate);
        _finderHsb      = new DialogOptionsFinderHsb(searchTemplate);
    }

    public bool IsDialogMode(params Bitmap[] image)
    {
        var isDialogIndication = _searchTemplate.DialogIndicationColorRange.Any(color => _bitmapUtils.CountInRange(image[0], color) > 0);
        Debug.WriteLine("Is Indication: " + isDialogIndication);

        var (firstLineY, inRangeCount) = _bitmapUtils.GetFirstLineAndCountInRange(image[1], _searchTemplate.SpeakerColorRangeRgb);
        var isSpeakerNamePresent = inRangeCount > _searchTemplate.SpeakerNameThreshold;
        Debug.WriteLine("Dialog Option. Position: " + firstLineY + " | PixelsCount: " + inRangeCount);

        if (!isDialogIndication && !isSpeakerNamePresent) return false;

        _speakerNameFirstLineY = isSpeakerNamePresent
            ? _searchTemplate.SpeakerNameArea.Height.From + firstLineY
            : image[1].Height;

        return true;
    }

    public List<Rectangle> GetDialogOptions(Bitmap image)
    {
        var dialogOptionsList = new List<Rectangle>();
        var dialogOptionsDebugList = new List<Rectangle>();

        image.Save("Debug/Crop.png");

        for (var y = 0; y <= _speakerNameFirstLineY - 1; y++)
        {
            //Debug.WriteLine(y);

            var (colorWrapperRgb, colorWrapperHsb) = GetColorWrappers(image, y);

            if (colorWrapperRgb is not null)
            {
                FindAndAddDialogOptionRectangle(
                    image,
                    colorWrapperRgb,
                    ref y,
                    _speakerNameFirstLineY,
                    dialogOptionsList,
                    dialogOptionsDebugList);

                continue;
            }

            if (colorWrapperHsb is not null)
            {
                FindAndAddDialogOptionRectangle(
                    image,
                    colorWrapperHsb,
                    ref y,
                    _speakerNameFirstLineY,
                    dialogOptionsList,
                    dialogOptionsDebugList);
            }
        }

        _bitmapUtils.DrawRectangles(image, dialogOptionsDebugList);
        image.Save("Debug/Result.png");

        return dialogOptionsList;
    }

    private (ColorWrapper<Rgba>?, ColorWrapper<Hsba>?) GetColorWrappers(Bitmap image, int y)
    {
        var colorWrapperRgb = GetColorWrapperRgb(image, y);
        if (colorWrapperRgb is not null) return (colorWrapperRgb, null);

        var colorWrapperHsb = GetColorWrapperHsb(image, y);
        if (colorWrapperHsb is null) return (null, null);
        return (null, colorWrapperHsb);
    }

    private ColorWrapper<Rgba>? GetColorWrapperRgb(Bitmap image, int y)
    {
        var colorWrapper = _searchTemplate.DialogOptionColorRanges.FirstOrDefault(colorWrapper =>
            _bitmapUtils.CountInRange(
                image,
                colorWrapper.IconColor,
                _searchTemplate.IconHorizontalRange.From,
                y,
                _searchTemplate.IconHorizontalRange.From + _searchTemplate.IconHorizontalRange.Length - 1,
                y)
            > 0);

        return colorWrapper;
    }
    private ColorWrapper<Hsba>? GetColorWrapperHsb(Bitmap image, int y)
    {
        var isDimmedColorRange = _bitmapUtils.CountInRange(image,
            _searchTemplate.DialogOptionDimmed.IconColor,
            _searchTemplate.IconHorizontalRange.From,
            y,
            _searchTemplate.IconHorizontalRange.From + _searchTemplate.IconHorizontalRange.Length - 1,
            y) > 0;

        return isDimmedColorRange ? _searchTemplate.DialogOptionDimmed : null;
    }

    private void FindAndAddDialogOptionRectangle(Bitmap image, ColorWrapper<Rgba> colorWrapper, ref int y, int maxY, ICollection<Rectangle> dialogOptions, ICollection<Rectangle> dialogOptionsDebug) =>
        _finderRgb.FindAndAddDialogOptionRectangle(image, colorWrapper, ref y, maxY, dialogOptions, dialogOptionsDebug);

    private void FindAndAddDialogOptionRectangle(Bitmap image, ColorWrapper<Hsba> colorWrapper, ref int y, int maxY, ICollection<Rectangle> dialogOptions, ICollection<Rectangle> dialogOptionsDebug) =>
        _finderHsb.FindAndAddDialogOptionRectangle(image, colorWrapper, ref y, maxY, dialogOptions, dialogOptionsDebug);
}
