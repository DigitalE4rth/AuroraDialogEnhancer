using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.Proxy;
using AuroraDialogEnhancerExtensions.Services;
using Extension.GenshinImpact.Templates;

namespace Extension.GenshinImpact.Services;

public class DialogOptionFinder : IDialogOptionFinder
{
    private readonly BitmapUtils _bitmapUtils;
    private readonly PreciseTemplate _searchTemplate;
    
    public DialogOptionFinder(PreciseTemplate searchTemplate)
    {
        _searchTemplate = searchTemplate;
        _bitmapUtils = new BitmapUtils();
    }

    public bool IsDialogMode(Bitmap image)
    {
        return _bitmapUtils.CountInRange(image, _searchTemplate.SpeakerColorRange) > _searchTemplate.SpeakerNameThreshold;
    }

    public List<Rectangle> GetDialogOptions(Bitmap image) => GetDialogOptions(image, _searchTemplate.Threshold);

    private List<Rectangle> GetDialogOptions(Bitmap image, double threshold)
    {
        var dialogOptionsList = new List<Rectangle>();
        using var croppedArea = _bitmapUtils.GetArea(
            image, 
            new Rectangle(
                _searchTemplate.DialogOptionsSearchArea.Width.From,
                _searchTemplate.DialogOptionsSearchArea.Height.From,
                _searchTemplate.DialogOptionsSearchArea.Width.Length,
                _searchTemplate.DialogOptionsSearchArea.Height.Length));

        /*using var croppedArea = _bitmapUtils.GetArea(
            rawImage,
            new Rectangle(
                0,
                0,
                rawImage.Width,
                rawImage.Height));*/

        using var croppedImage = _bitmapUtils.ToGrayScale(croppedArea);

        croppedImage.Save("Cut.png");

        for (var x = 0; x + _searchTemplate.DialogOptionsSearchArea.Width.Length <= croppedImage.Width; x++)
        {
            var isFound = false;
            for (var y = 0; y < croppedImage.Height; y++)
            {
                //if(y >= 620) 
                 //   System.Diagnostics.Debug.WriteLine("a");

                #region Top outline
                var (isTopFound, topOutline) = GetTopOutline(croppedImage, x, y, threshold);
                if (isTopFound == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Bottom outline
                var (isBottomFound, bottomOutline) = GetBottomOutline(croppedImage, x, topOutline, threshold);
                if (isBottomFound == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Empty center
                if (GetYOutline(croppedImage, 
                        x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
                        x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
                        topOutline + _searchTemplate.CenterOutlineSearchRangeY.From,
                        topOutline + _searchTemplate.CenterOutlineSearchRangeY.To,
                        0.5) != -1)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Left outline
                var leftOutline = GetXOutline(croppedImage,
                    x + _searchTemplate.VerticalOutlineSearchRangeX.From,
                    y + _searchTemplate.VerticalOutlineSearchRangeY.From,
                    x + _searchTemplate.VerticalOutlineSearchRangeX.To,
                    y + _searchTemplate.VerticalOutlineSearchRangeY.To,
                    threshold);
                if (leftOutline == -1)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Result wrapping
                // Test area
                var area = new Rectangle(
                    leftOutline,
                    topOutline,
                    _searchTemplate.Width,
                    bottomOutline - topOutline);
                dialogOptionsList.Add(area);

                    //Client Area
                    /* 
                    var area = new Rectangle(
                        verticalOutline - _searchTemplate.BackgroundPadding,
                        topOutline - _searchTemplate.BackgroundPadding,
                        _searchTemplate.Width,
                        bottomOutline - topOutline + _searchTemplate.BackgroundPadding * 2);
                    dialogOptionsList.Add(area);
                    */
                    #endregion

                y = area.Bottom + _searchTemplate.Gap;
                isFound = true;
            }

            x = isFound 
                ? x + _searchTemplate.DialogOptionsSearchArea.Width.Length 
                : x + _searchTemplate.VerticalOutlineSearchRangeX.Length;
        }

        _bitmapUtils.DrawRectangles(croppedImage, dialogOptionsList);
        croppedImage.Save("Result.png");

        return dialogOptionsList;
    }

    #region Utils
    private (bool, int) GetTopOutline(Bitmap image, int x, int y, double threshold)
    {
        var maxSearchPoint = y + _searchTemplate.TopOutlineSearchRangeY.To;
        if (maxSearchPoint >= image.Height) return (false, maxSearchPoint);

        var firstOutline = GetYOutline(image,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
            y + _searchTemplate.TopOutlineSearchRangeY.From,
            maxSearchPoint, threshold);
        if (firstOutline == -1) return (false, y + _searchTemplate.TopOutlineSearchRangeY.To);

        var nextOrdinal = GetYOrdinal(image,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
            firstOutline + 1,
            maxSearchPoint, threshold);
        if (nextOrdinal == -1) return (true, maxSearchPoint);
        
        var nextOutline = GetYOutline(image,
                x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
                x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
                nextOrdinal,
                maxSearchPoint, 0.6);
        return nextOutline == -1 ? (true, firstOutline) : (false, nextOutline);
    }

    private (bool, int) GetBottomOutline(Bitmap image, int x, int y, double threshold)
    {
        var maxSearchPoint = y + _searchTemplate.BottomOutlineSearchRangeY.To;
        if (maxSearchPoint >= image.Height) return (false, maxSearchPoint);

        var firstOutlinePosition = GetYOutline(image,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
            x +_searchTemplate.HorizontalOutlineSearchRangeX.To,
            y + _searchTemplate.BottomOutlineSearchRangeY.From,
            maxSearchPoint, threshold);

        if (firstOutlinePosition == -1) return (false, y);

        var ordinalLinePosition = GetYOrdinal(image,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
            firstOutlinePosition + 1,
            maxSearchPoint, threshold);

        return (true, ordinalLinePosition == -1 ? maxSearchPoint : ordinalLinePosition - 1);
    }

    private int GetYOutline(Bitmap image, int x, int maxX, int y, int maxY, double threshold)
    {
        for (var i = y; i <= maxY; i++)
        {
            if (IsHorizontalOutline(image, x, i, maxX, threshold)) return i;
        }
        return -1;
    }

    private int GetYOrdinal(Bitmap image, int x, int maxX, int y, int maxY, double threshold)
    {
        for (var i = y; i <= maxY; i++)
        {
            if (IsHorizontalOutline(image, x, i, maxX, threshold) == false) return i;
        }
        return -1;
    }

    private bool IsHorizontalOutline(Bitmap image, int x, int y, int maxX, double threshold)
    {
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            if (_bitmapUtils.IsWithinGrayRange(image, x, y, _searchTemplate.GrayChannelRange)) resultPoints++;
        }

        return resultPoints >= (maxX - x) * threshold;
    }

    private int GetXOutline(Bitmap image, int x, int y, int maxX, int maxY, double threshold)
    {
        while (maxX >= 0)
        {
            if (IsVerticalOutline(image, maxX, y, maxY, threshold) == false)
            {
                maxX -= 1;
                continue;
            }

            var ordinal = GetXOrdinalBackward(image, x, maxX, y, maxY, threshold);
            return ordinal == -1 ? x : ordinal + 1;
        }

        return -1;
    }

    private int GetXOrdinalBackward(Bitmap image, int x, int maxX, int y, int maxY, double threshold)
    {
        for (var i = maxX; i >= x; i--)
        {
            if (IsVerticalOutline(image, i, y, maxY, threshold) == false) return i;
        }

        return -1;
    }

    private bool IsVerticalOutline(Bitmap image, int x, int y, int maxY, double threshold)
    {
        var totalPoints = maxY - y;
        var resultPoints = 0;

        for (var i = y; i < maxY; i++)
        {
            if (_bitmapUtils.IsWithinGrayRange(image, x, y, _searchTemplate.GrayChannelRange)) resultPoints++;
        }

        return resultPoints >= totalPoints * threshold;
    }
    #endregion
}
