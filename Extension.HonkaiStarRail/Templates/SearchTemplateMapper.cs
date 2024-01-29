using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

internal class SearchTemplateMapper
{
    private readonly DynamicTemplateFactory _dynamicTemplateFactory = new();

    public SearchTemplate Map(Size clientSize)
    {
        var dynamicTemplate = _dynamicTemplateFactory.GetTemplate(clientSize);
        var searchTemplate = new SearchTemplate();

        #region Dialog
        searchTemplate.DialogIndicationColorRange = dynamicTemplate.DialogIndicationColorRange;
        searchTemplate.DialogIndicationArea = new Area(
            (int)(dynamicTemplate.DialogIndicationArea.Width.From * clientSize.Width),
            (int)(dynamicTemplate.DialogIndicationArea.Width.To * clientSize.Width),
            (int)(dynamicTemplate.DialogIndicationArea.Height.From * clientSize.Height),
            (int)(dynamicTemplate.DialogIndicationArea.Height.To * clientSize.Height));

        searchTemplate.SpeakerColorRangeRgb = dynamicTemplate.SpeakerColorRangeRgb;
        searchTemplate.SpeakerNameArea = new Area(
            (int)(dynamicTemplate.SpeakerNameArea.Width.From * clientSize.Width),
            (int)(dynamicTemplate.SpeakerNameArea.Width.To * clientSize.Width),
            (int)(dynamicTemplate.SpeakerNameArea.Height.From * clientSize.Height),
            (int)(dynamicTemplate.SpeakerNameArea.Height.To * clientSize.Height));

        searchTemplate.SpeakerNameThreshold = (int)(searchTemplate.SpeakerNameArea.Width.Length * searchTemplate.SpeakerNameArea.Height.Length * dynamicTemplate.SpeakerNameThreshold);
        #endregion

        #region Measurements
        searchTemplate.TemplateSearchArea = new Area(
            (int)(dynamicTemplate.TemplateSearchArea.Width.From * clientSize.Width),
            (int)(dynamicTemplate.TemplateSearchArea.Width.To * clientSize.Width),
            (int)(dynamicTemplate.TemplateSearchArea.Height.From * clientSize.Height),
            (int)(dynamicTemplate.TemplateSearchArea.Height.To * clientSize.Height));

        searchTemplate.DialogOptionWidth = (int)(dynamicTemplate.DialogOptionWidth * clientSize.Width);

        searchTemplate.Gap = (int) (clientSize.Height * dynamicTemplate.Gap);
        searchTemplate.GapHalf = searchTemplate.Gap / 2;
        #endregion

        #region Icon
        searchTemplate.IconMinLength = (int) (clientSize.Height * dynamicTemplate.IconMinLength);
        searchTemplate.IconMaxLength = (int) (clientSize.Height * dynamicTemplate.IconMaxLength);

        searchTemplate.IconThreshold = dynamicTemplate.IconThreshold;

        var iconHorizontalRangeFrom = (int) (clientSize.Width * dynamicTemplate.IconHorizontalRange.From);
        var iconHorizontalRangeTo   = (int) (clientSize.Width * dynamicTemplate.IconHorizontalRange.To);
        var iconHorizontalRangeLength = iconHorizontalRangeTo - iconHorizontalRangeFrom;
        var relatedIconHorizontalRangeFrom = iconHorizontalRangeFrom - searchTemplate.TemplateSearchArea.Width.From;
        searchTemplate.IconHorizontalRange = new Range(
            relatedIconHorizontalRangeFrom,
            relatedIconHorizontalRangeFrom + iconHorizontalRangeLength);

        searchTemplate.IconClearAreaSearchOffset = dynamicTemplate.IconClearAreaSearchOffset;
        #endregion

        #region Text
        var textHorizontalRangeFrom = (int) (clientSize.Width * dynamicTemplate.TextHorizontalRange.From);
        var textHorizontalRangeTo   = (int) (clientSize.Width * dynamicTemplate.TextHorizontalRange.To);
        var textHorizontalRangeLength = textHorizontalRangeTo - textHorizontalRangeFrom;
        var relatedTextHorizontalRangeFrom = textHorizontalRangeFrom - searchTemplate.TemplateSearchArea.Width.From;
        searchTemplate.TextHorizontalRange = new Range(
            relatedTextHorizontalRangeFrom,
            relatedTextHorizontalRangeFrom + textHorizontalRangeLength);

        searchTemplate.TextLineHeight = (int) (clientSize.Height * dynamicTemplate.TextLineHeight);
        searchTemplate.TextLineHeightHalf = searchTemplate.TextLineHeight / 2;

        searchTemplate.TextSingleTopBottomMargin   = (int) (dynamicTemplate.TextSingleTopBottomMargin   * searchTemplate.TextLineHeight);
        searchTemplate.TextMultipleTopBottomMargin = (int) (dynamicTemplate.TextMultipleTopBottomMargin * searchTemplate.TextLineHeight);
        #endregion

        #region Colors
        searchTemplate.DialogOptionColorRanges = dynamicTemplate.DialogOptionColorRanges;
        searchTemplate.DialogOptionDimmed      = dynamicTemplate.DialogOptionDimmed;
        #endregion

        return searchTemplate;
    }
}
