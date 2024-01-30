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
    private readonly SearchTemplate              _searchTemplate;
    private readonly BitmapUtils                 _bitmapUtils;
    private readonly DialogOptionsFinderRgbDebug _finderRgb;
    private readonly DialogOptionsFinderHsbDebug _finderHsb;
    private readonly List<Rectangle>             _dialogOptionsList;
    private readonly List<Rectangle>             _dialogOptionsDebugList;
    private int                                  _speakerNameFirstLineY;

    public DialogOptionFinderDebug(SearchTemplate searchTemplate)
    {
        _searchTemplate         = searchTemplate;
        _bitmapUtils            = new BitmapUtils();
        _finderRgb              = new DialogOptionsFinderRgbDebug(_bitmapUtils, searchTemplate);
        _finderHsb              = new DialogOptionsFinderHsbDebug(_bitmapUtils, searchTemplate);
        _dialogOptionsList      = new List<Rectangle>();
        _dialogOptionsDebugList = new List<Rectangle>();
    }

    public bool IsDialogMode(params Bitmap[] image)
    {
        var isIndicationPresent        = _searchTemplate.DialogIndicationColorRange.Any(color => _bitmapUtils.IsImageContainsColor(image[0], color));
        var isEmptyIndicationAreaEmpty = !_searchTemplate.DialogIndicationEmptyColorRange.Any(color => _bitmapUtils.IsImageContainsColor(image[1], color));
        image[0].Save("Debug/Ind.png");
        image[1].Save("Debug/Empty.png");
        var (firstLineY, inRangeCount) = _bitmapUtils.GetFirstLineAndCountInRange(image[2], _searchTemplate.SpeakerColorRangeRgb);
        var isSpeakerNamePresent = inRangeCount > _searchTemplate.SpeakerNameThreshold;

        if (!(isIndicationPresent && isEmptyIndicationAreaEmpty) && !isSpeakerNamePresent) return false;

        _speakerNameFirstLineY = isSpeakerNamePresent
            ? _searchTemplate.SpeakerNameArea.Height.From + firstLineY
            : image[1].Height;

        return true;
    }

    public List<Rectangle> GetDialogOptions(Bitmap image)
    {
        var watch = Stopwatch.StartNew();
        _dialogOptionsList.Clear();
        _dialogOptionsDebugList.Clear();

        image.Save("Debug/Crop.png");

        for (var y = 0; y <= _speakerNameFirstLineY - 1; y++)
        {
            //Debug.WriteLine(y);
            if (y == 492)
            {
                Debug.WriteLine("A");
            }

            var colorWrapperRgb = GetColorWrapperRgb(image, y);
            if (colorWrapperRgb is not null)
            {
                _finderRgb.FindAndAddDialogOptionRectangle(
                    image,
                    colorWrapperRgb,
                    ref y,
                    _speakerNameFirstLineY,
                    _dialogOptionsList,
                    _dialogOptionsDebugList);

                continue;
            }

            var colorWrapperHsb = GetColorWrapperHsb(image, y);
            if (colorWrapperHsb is not null)
            {
                _finderHsb.FindAndAddDialogOptionRectangle(
                    image,
                    colorWrapperHsb,
                    ref y,
                    _speakerNameFirstLineY,
                    _dialogOptionsList,
                    _dialogOptionsDebugList);
            }
        }

        _bitmapUtils.DrawRectangles(image, _dialogOptionsDebugList);
        image.Save("Debug/Result.png");

        watch.Stop();
        Debug.WriteLine("Time: " + watch.ElapsedMilliseconds);

        return _dialogOptionsList;
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
}
