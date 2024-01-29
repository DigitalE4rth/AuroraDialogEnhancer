using System;
using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

internal class SearchTemplateMapper
{
    private readonly DynamicTemplateFactory _dynamicTemplateFactory = new();

    public PreciseTemplate Map(Size clientSize)
    {
        var dynamicTemplate = _dynamicTemplateFactory.GetTemplate(clientSize);

        var preciseTemplate = new PreciseTemplate();

        #region Speaker
        preciseTemplate.SpeakerColorRangeRgb = dynamicTemplate.SpeakerColorRangeRgb;

        preciseTemplate.SpeakerNameArea = new Area(
            (int)(dynamicTemplate.SpeakerNameArea.Width.From * clientSize.Width),
            (int)(dynamicTemplate.SpeakerNameArea.Width.To * clientSize.Width),
            (int)(dynamicTemplate.SpeakerNameArea.Height.From * clientSize.Height),
            (int)(dynamicTemplate.SpeakerNameArea.Height.To * clientSize.Height));

        preciseTemplate.SpeakerNameThreshold = (int)(preciseTemplate.SpeakerNameArea.Width.Length * preciseTemplate.SpeakerNameArea.Height.Length * dynamicTemplate.SpeakerNameThreshold);
        #endregion

        #region Measurements
        var templateWidth = (int)(dynamicTemplate.TemplateWidth * clientSize.Width);

        var templateHeight = (int)(dynamicTemplate.TemplateHeight * clientSize.Width);

        preciseTemplate.TemplateSearchArea = new Area(
            (int)(dynamicTemplate.TemplateSearchArea.Width.From * clientSize.Width),
            (int)(dynamicTemplate.TemplateSearchArea.Width.To * clientSize.Width),
            (int)(dynamicTemplate.TemplateSearchArea.Height.From * clientSize.Height),
            (int)(dynamicTemplate.TemplateSearchArea.Height.To * clientSize.Height));

        preciseTemplate.DialogOptionWidth = (int)(dynamicTemplate.DialogOptionWidth * clientSize.Width);

        preciseTemplate.DialogOptionHeight = (int)(dynamicTemplate.DialogOptionHeight * clientSize.Width);

        preciseTemplate.Gap = (int)(clientSize.Width * dynamicTemplate.Gap);

        preciseTemplate.BackgroundPadding = (int)Math.Ceiling(clientSize.Width * dynamicTemplate.BackgroundPadding);

        preciseTemplate.OutlineAreaHeight = (int)(preciseTemplate.DialogOptionHeight * dynamicTemplate.OutlineAreaHeight);
        #endregion

        #region Lines
        preciseTemplate.OutlineGrayChannelRange = dynamicTemplate.OutlineGrayChannelRange;

        preciseTemplate.VerticalOutlineSearchRangeX = new Range(
            (int)(templateWidth * dynamicTemplate.VerticalOutlineSearchRangeX.From),
            (int)(templateWidth * dynamicTemplate.VerticalOutlineSearchRangeX.To));

        preciseTemplate.VerticalOutlineSearchRangeY = new Range(
            (int)(templateHeight * dynamicTemplate.VerticalOutlineSearchRangeY.From),
            (int)(templateHeight * dynamicTemplate.VerticalOutlineSearchRangeY.To));

        preciseTemplate.VerticalOutlineThreshold = (int)(preciseTemplate.VerticalOutlineSearchRangeY.Length * dynamicTemplate.VerticalOutlineThreshold);

        preciseTemplate.HorizontalOutlineSearchRangeX = new Range(
            (int)(templateWidth * dynamicTemplate.HorizontalOutlineSearchRangeX.From),
            (int)(templateWidth * dynamicTemplate.HorizontalOutlineSearchRangeX.To));

        preciseTemplate.HorizontalOutlineThreshold = (int)(preciseTemplate.HorizontalOutlineSearchRangeX.Length * dynamicTemplate.HorizontalOutlineThreshold);

        preciseTemplate.TopOutlineSearchRangeY = new Range(
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.TopOutlineSearchRangeY.From),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.TopOutlineSearchRangeY.To));

        preciseTemplate.BottomOutlineSearchRangeY = new Range(
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.BottomOutlineSearchRangeY.From),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.BottomOutlineSearchRangeY.To));

        foreach (var cornerArea in dynamicTemplate.CornerOutlineAreas)
        {
            var widthFrom = (int)(cornerArea.Width.From * templateWidth);
            var widthTo = (int)(cornerArea.Width.To * templateWidth);
            var heightFrom = (int)(cornerArea.Height.From * templateHeight);
            var heightTo = (int)(cornerArea.Height.To * templateHeight);
            var threshold = (int) Math.Ceiling((widthTo - widthFrom) * (heightTo - heightFrom) * cornerArea.Threshold);

            preciseTemplate.CornerOutlineAreas.Add(new ThresholdArea(widthFrom, widthTo, heightFrom, heightTo, threshold));
        }
        #endregion

        #region Extra
        preciseTemplate.IconArea = new Area(
            (int)(templateWidth * dynamicTemplate.IconArea.Width.From),
            (int)(templateWidth * dynamicTemplate.IconArea.Width.To),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.IconArea.Height.From),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.IconArea.Height.To));

        preciseTemplate.IconColorRanges = dynamicTemplate.IconColorRanges;

        preciseTemplate.IconAreaThreshold = (int)(preciseTemplate.IconArea.Width.Length * preciseTemplate.IconArea.Height.Length * dynamicTemplate.IconAreaThreshold);

        preciseTemplate.EmptyCenterArea = new Area(
            (int)(templateWidth * dynamicTemplate.EmptyCenterArea.Width.From),
            (int)(templateWidth * dynamicTemplate.EmptyCenterArea.Width.To),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.EmptyCenterArea.Height.From),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.EmptyCenterArea.Height.To));

        preciseTemplate.EmptyCenterAreaThreshold = (int)(preciseTemplate.EmptyCenterArea.Width.Length * preciseTemplate.EmptyCenterArea.Height.Length * dynamicTemplate.EmptyCenterAreaThreshold);
        #endregion

        return preciseTemplate;
    }
}
