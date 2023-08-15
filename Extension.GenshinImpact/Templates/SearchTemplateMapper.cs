using System;
using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

internal class SearchTemplateMapper
{
    public PreciseTemplate Map(Size clientSize)
    {
        var dynamicSearchTemplate = new DynamicTemplate();
        var searchTemplate = new PreciseTemplate();

        searchTemplate.SpeakerColorRange = dynamicSearchTemplate.SpeakerColorRange;

        searchTemplate.SpeakerNameArea = new Area(
            (int)(dynamicSearchTemplate.SpeakerNameArea.Width.From * clientSize.Width),
            (int)(dynamicSearchTemplate.SpeakerNameArea.Width.To * clientSize.Width),
            (int)(dynamicSearchTemplate.SpeakerNameArea.Height.From * clientSize.Height),
            (int)(dynamicSearchTemplate.SpeakerNameArea.Height.To * clientSize.Height));

        searchTemplate.SpeakerNameThreshold = (int)(searchTemplate.SpeakerNameArea.Width.Length * searchTemplate.SpeakerNameArea.Height.Length * dynamicSearchTemplate.SpeakerNameThreshold);

        searchTemplate.DialogOptionsSearchArea = new Area(
            (int)(dynamicSearchTemplate.DialogOptionsSearchArea.Width.From * clientSize.Width),
            (int)(dynamicSearchTemplate.DialogOptionsSearchArea.Width.To * clientSize.Width),
            (int)(dynamicSearchTemplate.DialogOptionsSearchArea.Height.From * clientSize.Height),
            (int)(dynamicSearchTemplate.DialogOptionsSearchArea.Height.To * clientSize.Height));

        searchTemplate.Width = (int)(dynamicSearchTemplate.Width * clientSize.Width);
        searchTemplate.Height = (int)(dynamicSearchTemplate.Height * clientSize.Width);
        
        var searchWidth = searchTemplate.DialogOptionsSearchArea.Width.To - searchTemplate.DialogOptionsSearchArea.Width.From;

        searchTemplate.VerticalOutlineSearchRangeX = new Range(
            (int)(searchWidth * dynamicSearchTemplate.VerticalOutlineSearchRangeX.From),
            (int)(searchWidth * dynamicSearchTemplate.VerticalOutlineSearchRangeX.To));

        searchTemplate.VerticalOutlineSearchRangeY = new Range(
            (int)(searchTemplate.Height * dynamicSearchTemplate.VerticalOutlineSearchRangeY.From),
            (int)(searchTemplate.Height * dynamicSearchTemplate.VerticalOutlineSearchRangeY.To));

        searchTemplate.HorizontalOutlineSearchRangeX = new Range(
            (int)(searchWidth * dynamicSearchTemplate.HorizontalOutlineSearchRangeX.From),
            (int)(searchWidth * dynamicSearchTemplate.HorizontalOutlineSearchRangeX.To));

        searchTemplate.BackgroundPadding = (int)Math.Ceiling(clientSize.Width * dynamicSearchTemplate.BackgroundPadding);

        searchTemplate.Gap = (int)(clientSize.Width * dynamicSearchTemplate.Gap);

        searchTemplate.OutlineAreaHeight = (int)(searchTemplate.Height * dynamicSearchTemplate.OutlineAreaHeight);

        searchTemplate.TopOutlineSearchRangeY = new Range(
            (int)(searchTemplate.OutlineAreaHeight * dynamicSearchTemplate.TopOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * dynamicSearchTemplate.TopOutlineSearchRangeY.To));

        searchTemplate.CenterOutlineSearchRangeY = new Range(
            (int)(searchTemplate.OutlineAreaHeight * dynamicSearchTemplate.CenterOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * dynamicSearchTemplate.CenterOutlineSearchRangeY.To));

        searchTemplate.BottomOutlineSearchRangeY = new Range(
            (int)(searchTemplate.OutlineAreaHeight * dynamicSearchTemplate.BottomOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * dynamicSearchTemplate.BottomOutlineSearchRangeY.To));

        searchTemplate.GrayChannelRange = dynamicSearchTemplate.GrayChannelRange;

        searchTemplate.Threshold = dynamicSearchTemplate.Threshold;

        return searchTemplate;
    }
}
