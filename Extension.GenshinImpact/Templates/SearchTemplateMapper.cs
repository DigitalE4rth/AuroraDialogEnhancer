using System;
using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

internal class SearchTemplateMapper
{
    public SearchTemplate Get(Size clientSize) => Get(clientSize, new DynamicTemplate());

    public SearchTemplate Get(Size clientSize, DynamicTemplate dynamicTemplate)
    {
        var searchTemplate = new SearchTemplate();

        searchTemplate.SpeakerColorRange = dynamicTemplate.SpeakerColorRange;

        searchTemplate.SpeakerNameArea = new Area(
            (int)(dynamicTemplate.SpeakerNameArea.Width.From * clientSize.Width),
            (int)(dynamicTemplate.SpeakerNameArea.Width.To * clientSize.Width),
            (int)(dynamicTemplate.SpeakerNameArea.Height.From * clientSize.Height),
            (int)(dynamicTemplate.SpeakerNameArea.Height.To * clientSize.Height));

        searchTemplate.SpeakerNameThreshold = (int)(searchTemplate.SpeakerNameArea.Width.Length * searchTemplate.SpeakerNameArea.Height.Length * dynamicTemplate.SpeakerNameThreshold);

        searchTemplate.DialogOptionsSearchArea = new Area(
            (int)(dynamicTemplate.DialogOptionsSearchArea.Width.From * clientSize.Width),
            (int)(dynamicTemplate.DialogOptionsSearchArea.Width.To * clientSize.Width),
            (int)(dynamicTemplate.DialogOptionsSearchArea.Height.From * clientSize.Height),
            (int)(dynamicTemplate.DialogOptionsSearchArea.Height.To * clientSize.Height));

        searchTemplate.Width = (int)(dynamicTemplate.Width * clientSize.Width);
        searchTemplate.Height = (int)(dynamicTemplate.Height * clientSize.Width);
        
        var searchWidth = searchTemplate.DialogOptionsSearchArea.Width.To - searchTemplate.DialogOptionsSearchArea.Width.From;

        searchTemplate.VerticalOutlineSearchRangeX = new Range(
            (int)(searchWidth * dynamicTemplate.VerticalOutlineSearchRangeX.From),
            (int)(searchWidth * dynamicTemplate.VerticalOutlineSearchRangeX.To));

        searchTemplate.VerticalOutlineSearchRangeY = new Range(
            (int)(searchTemplate.Height * dynamicTemplate.VerticalOutlineSearchRangeY.From),
            (int)(searchTemplate.Height * dynamicTemplate.VerticalOutlineSearchRangeY.To));

        searchTemplate.HorizontalOutlineSearchRangeX = new Range(
            (int)(searchWidth * dynamicTemplate.HorizontalOutlineSearchRangeX.From),
            (int)(searchWidth * dynamicTemplate.HorizontalOutlineSearchRangeX.To));

        searchTemplate.BackgroundPadding = (int)Math.Ceiling(clientSize.Width * dynamicTemplate.BackgroundPadding);

        searchTemplate.Gap = (int)(clientSize.Width * dynamicTemplate.Gap);

        searchTemplate.OutlineAreaHeight = (int)(searchTemplate.Height * dynamicTemplate.OutlineAreaHeight);

        searchTemplate.TopOutlineSearchRangeY = new Range(
            (int)(searchTemplate.OutlineAreaHeight * dynamicTemplate.TopOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * dynamicTemplate.TopOutlineSearchRangeY.To));

        searchTemplate.CenterOutlineSearchRangeY = new Range(
            (int)(searchTemplate.OutlineAreaHeight * dynamicTemplate.CenterOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * dynamicTemplate.CenterOutlineSearchRangeY.To));

        searchTemplate.BottomOutlineSearchRangeY = new Range(
            (int)(searchTemplate.OutlineAreaHeight * dynamicTemplate.BottomOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * dynamicTemplate.BottomOutlineSearchRangeY.To));

        searchTemplate.GrayChannelRange = dynamicTemplate.GrayChannelRange;

        searchTemplate.Threshold = dynamicTemplate.Threshold;

        return searchTemplate;
    }
}
