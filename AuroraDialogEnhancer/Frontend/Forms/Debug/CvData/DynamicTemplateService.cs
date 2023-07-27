using System;
using System.Drawing;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug.CvData;

internal class DynamicTemplateService
{
    private readonly DynamicTemplate _dynamicTemplate;

    public DynamicTemplateService()
    {
        _dynamicTemplate = new DynamicTemplate();
    }

    public SearchTemplate GetDynamicTemplate(Size clientSize)
    {
        var searchTemplate = new SearchTemplate();
        searchTemplate.DialogOptionsSearchArea = new SearchArea<int>(
            (int)(_dynamicTemplate.DialogOptionsSearchArea.Width.From * clientSize.Width),
            (int)(_dynamicTemplate.DialogOptionsSearchArea.Width.To * clientSize.Width),
            (int)(_dynamicTemplate.DialogOptionsSearchArea.Height.From * clientSize.Height),
            (int)(_dynamicTemplate.DialogOptionsSearchArea.Height.To * clientSize.Height));

        searchTemplate.Width = (int)(_dynamicTemplate.Width * clientSize.Width);
        searchTemplate.Height = (int)(_dynamicTemplate.Height * clientSize.Width);
        
        var searchWidth = searchTemplate.DialogOptionsSearchArea.Width.To - searchTemplate.DialogOptionsSearchArea.Width.From;

        searchTemplate.VerticalOutlineSearchRangeX = new SearchRange<int>(
            (int)(searchWidth * _dynamicTemplate.VerticalOutlineSearchRangeX.From),
            (int)(searchWidth * _dynamicTemplate.VerticalOutlineSearchRangeX.To));

        searchTemplate.VerticalOutlineSearchRangeY = new SearchRange<int>(
            (int)(searchTemplate.Height * _dynamicTemplate.VerticalOutlineSearchRangeY.From),
            (int)(searchTemplate.Height * _dynamicTemplate.VerticalOutlineSearchRangeY.To));

        searchTemplate.HorizontalOutlineSearchRangeX = new SearchRange<int>(
            (int)(searchWidth * _dynamicTemplate.HorizontalOutlineSearchRangeX.From),
            (int)(searchWidth * _dynamicTemplate.HorizontalOutlineSearchRangeX.To));

        searchTemplate.BackgroundPadding = (int)Math.Ceiling(clientSize.Width * _dynamicTemplate.BackgroundPadding);

        searchTemplate.Gap = (int)(clientSize.Width * _dynamicTemplate.Gap);

        searchTemplate.OutlineAreaHeight = (int)(searchTemplate.Height * _dynamicTemplate.OutlineAreaHeight);

        searchTemplate.TopOutlineSearchRangeY = new SearchRange<int>(
            (int)(searchTemplate.OutlineAreaHeight * _dynamicTemplate.TopOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * _dynamicTemplate.TopOutlineSearchRangeY.To));

        searchTemplate.CenterOutlineSearchRangeY = new SearchRange<int>(
            (int)(searchTemplate.OutlineAreaHeight * _dynamicTemplate.CenterOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * _dynamicTemplate.CenterOutlineSearchRangeY.To));

        searchTemplate.BottomOutlineSearchRangeY = new SearchRange<int>(
            (int)(searchTemplate.OutlineAreaHeight * _dynamicTemplate.BottomOutlineSearchRangeY.From),
            (int)(searchTemplate.OutlineAreaHeight * _dynamicTemplate.BottomOutlineSearchRangeY.To));

        searchTemplate.GrayColorRange = _dynamicTemplate.GrayColorRange;

        return searchTemplate;
    }
}
