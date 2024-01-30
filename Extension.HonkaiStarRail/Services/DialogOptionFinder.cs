using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Proxy;
using AuroraDialogEnhancerExtensions.Services;
using Extension.HonkaiStarRail.Templates;
using Extension.HonkaiStarRail.Utils;

namespace Extension.HonkaiStarRail.Services;

public class DialogOptionFinder : IDialogOptionFinder
{
    private readonly SearchTemplate         _searchTemplate;
    private readonly BitmapUtils            _bitmapUtils;
    private readonly DialogOptionsFinderRgb _finderRgb;
    private readonly DialogOptionsFinderHsb _finderHsb;
    private readonly List<Rectangle>        _dialogOptionsList;
    private int                             _speakerNameFirstLineY;

    public DialogOptionFinder(SearchTemplate searchTemplate)
    {
        _searchTemplate    = searchTemplate;
        _bitmapUtils       = new BitmapUtils();
        _finderRgb         = new DialogOptionsFinderRgb(_bitmapUtils, searchTemplate);
        _finderHsb         = new DialogOptionsFinderHsb(_bitmapUtils, searchTemplate);
        _dialogOptionsList = new List<Rectangle>();
    }

    public bool IsDialogMode(params Bitmap[] image)
    {
        var isIndicationPresent        = _searchTemplate.DialogIndicationColorRange.Any(color => _bitmapUtils.IsImageContainsColor(image[0], color));
        var isEmptyIndicationAreaEmpty = !_searchTemplate.DialogIndicationEmptyColorRange.Any(color => _bitmapUtils.IsImageContainsColor(image[1], color));

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
        _dialogOptionsList.Clear();

        for (var y = 0; y <= _speakerNameFirstLineY - 1; y++)
        {
            var colorWrapperRgb = GetColorWrapperRgb(image, y);
            if (colorWrapperRgb is not null)
            {
                _finderRgb.FindAndAddDialogOptionRectangle(
                    image,
                    colorWrapperRgb,
                    ref y,
                    _speakerNameFirstLineY,
                    _dialogOptionsList);

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
                    _dialogOptionsList);
            }
        }

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
