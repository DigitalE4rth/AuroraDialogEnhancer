using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancerExtensions.Dimensions;
using Extension.GenshinImpact.Dimensions;

namespace Extension.GenshinImpact.Templates;

internal class SearchTemplateMapper
{
    public PreciseTemplate Map(Size clientSize)
    {
        var dynamicTemplate = new DynamicTemplate();
        var preciseTemplate = new PreciseTemplate();

        #region Speaker
        preciseTemplate.SpeakerColorRange = dynamicTemplate.SpeakerColorRange;

        preciseTemplate.SpeakerNameArea = new Area(
            (int)(dynamicTemplate.SpeakerNameArea.Width.From * clientSize.Width),
            (int)(dynamicTemplate.SpeakerNameArea.Width.To * clientSize.Width),
            (int)(dynamicTemplate.SpeakerNameArea.Height.From * clientSize.Height),
            (int)(dynamicTemplate.SpeakerNameArea.Height.To * clientSize.Height));

        preciseTemplate.SpeakerNameThreshold = (int) (preciseTemplate.SpeakerNameArea.Width.Length * preciseTemplate.SpeakerNameArea.Height.Length * dynamicTemplate.SpeakerNameThreshold);
        #endregion

        #region Measurements
        preciseTemplate.TemplateWidth = (int)(dynamicTemplate.TemplateWidth * clientSize.Width);

        preciseTemplate.TemplateHeight = (int)(dynamicTemplate.TemplateHeight * clientSize.Width);

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
            (int)(preciseTemplate.TemplateWidth * dynamicTemplate.VerticalOutlineSearchRangeX.From),
            (int)(preciseTemplate.TemplateWidth * dynamicTemplate.VerticalOutlineSearchRangeX.To));

        preciseTemplate.VerticalOutlineSearchRangeY = new Range(
            (int)(preciseTemplate.TemplateHeight * dynamicTemplate.VerticalOutlineSearchRangeY.From),
            (int)(preciseTemplate.TemplateHeight * dynamicTemplate.VerticalOutlineSearchRangeY.To));

        preciseTemplate.VerticalOutlineThreshold = (int)(preciseTemplate.VerticalOutlineSearchRangeY.Length * dynamicTemplate.VerticalOutlineThreshold);

        preciseTemplate.HorizontalOutlineSearchRangeX = new Range(
            (int)(preciseTemplate.TemplateWidth * dynamicTemplate.HorizontalOutlineSearchRangeX.From),
            (int)(preciseTemplate.TemplateWidth * dynamicTemplate.HorizontalOutlineSearchRangeX.To));

        preciseTemplate.HorizontalOutlineThreshold = (int)(preciseTemplate.HorizontalOutlineSearchRangeX.Length * dynamicTemplate.HorizontalOutlineThreshold);

        preciseTemplate.TopOutlineSearchRangeY = new Range(
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.TopOutlineSearchRangeY.From),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.TopOutlineSearchRangeY.To));

        preciseTemplate.BottomOutlineSearchRangeY = new Range(
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.BottomOutlineSearchRangeY.From),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.BottomOutlineSearchRangeY.To));

        preciseTemplate.CornerOutlineAreas = dynamicTemplate.CornerOutlineAreas.Select(cornerArea => new Area(
            (int)(cornerArea.Width.From * preciseTemplate.TemplateWidth),
            (int)(cornerArea.Width.To * preciseTemplate.TemplateWidth),
            (int)(cornerArea.Height.From * preciseTemplate.TemplateHeight),
            (int)(cornerArea.Height.To * preciseTemplate.TemplateHeight))).ToList();
        #endregion

        #region Extra
        preciseTemplate.IconArea = new Area(
            (int)(preciseTemplate.TemplateWidth * dynamicTemplate.IconArea.Width.From),
            (int)(preciseTemplate.TemplateWidth * dynamicTemplate.IconArea.Width.To),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.IconArea.Height.From),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.IconArea.Height.To));

        preciseTemplate.IconAreaThreshold = (int)(preciseTemplate.IconArea.Width.Length * preciseTemplate.IconArea.Height.Length * dynamicTemplate.IconAreaThreshold);

        preciseTemplate.EmptyCenterArea = new Area(
            (int)(preciseTemplate.TemplateWidth * dynamicTemplate.EmptyCenterArea.Width.From),
            (int)(preciseTemplate.TemplateWidth * dynamicTemplate.EmptyCenterArea.Width.To),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.EmptyCenterArea.Height.From),
            (int)(preciseTemplate.OutlineAreaHeight * dynamicTemplate.EmptyCenterArea.Height.To));

        preciseTemplate.EmptyCenterAreaThreshold = (int)(preciseTemplate.EmptyCenterArea.Width.Length * preciseTemplate.EmptyCenterArea.Height.Length * dynamicTemplate.EmptyCenterAreaThreshold);
        #endregion

        return preciseTemplate;
    }
}
