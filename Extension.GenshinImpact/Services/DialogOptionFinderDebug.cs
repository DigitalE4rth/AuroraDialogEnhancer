﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Proxy;
using AuroraDialogEnhancerExtensions.Services;
using Extension.GenshinImpact.Templates;

namespace Extension.GenshinImpact.Services;

public class DialogOptionFinderDebug : IDialogOptionFinder
{
    private readonly BitmapUtils     _bitmapUtils;
    private readonly PreciseTemplate _searchTemplate;
    
    public DialogOptionFinderDebug(PreciseTemplate searchTemplate)
    {
        _searchTemplate = searchTemplate;
        _bitmapUtils = new BitmapUtils();
    }

    public bool IsDialogMode(params Bitmap[] image)
    {
        return _bitmapUtils.CountInRange(image[0], _searchTemplate.SpeakerColorRangeRgb) > _searchTemplate.SpeakerNameThreshold;
    }

    public List<Rectangle> GetDialogOptions(Bitmap image)
    {
        var dialogOptionsList = new List<Rectangle>();
        var dialogOptionsDebugList = new List<Rectangle>();

        using var grayImage = _bitmapUtils.ToGrayScale(image);
        grayImage.Save("Debug/CroppedImage.png");
        for (var x = 0; x + _searchTemplate.TemplateSearchArea.Width.Length - 1 <= grayImage.Width - 1; x++)
        {
            var isFoundByY = false;
            for (var y = 0; y < grayImage.Height - 1; y++)
            {
                if (y > 675)
                {
                    Debug.WriteLine("");
                }

                #region Top outline
                var (isTopFound, topOutline) = GetTopOutline(grayImage, x, y, _searchTemplate.HorizontalOutlineThreshold);
                if (isTopFound == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Bottom outline
                var (isBottomFound, bottomOutline) = GetBottomOutline(grayImage, x, topOutline, _searchTemplate.HorizontalOutlineThreshold);
                if (isBottomFound == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Left outline
                _bitmapUtils.GetArea(
                        grayImage,
                        new Rectangle(
                            x + _searchTemplate.VerticalOutlineSearchRangeX.From,
                            y + _searchTemplate.VerticalOutlineSearchRangeY.From,
                            (x + _searchTemplate.VerticalOutlineSearchRangeX.To) - (x + _searchTemplate.VerticalOutlineSearchRangeX.From),
                            (y + _searchTemplate.VerticalOutlineSearchRangeY.To) - (y + _searchTemplate.VerticalOutlineSearchRangeY.From)))
                    .Save("Debug/LeftOutline.png");

                var leftOutline = GetXOutline(grayImage,
                    x + _searchTemplate.VerticalOutlineSearchRangeX.From,
                    y + _searchTemplate.VerticalOutlineSearchRangeY.From,
                    x + _searchTemplate.VerticalOutlineSearchRangeX.To,
                    y + _searchTemplate.VerticalOutlineSearchRangeY.To,
                    _searchTemplate.VerticalOutlineThreshold);
                if (leftOutline == -1)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Corners
                for (var index = 0; index < _searchTemplate.CornerOutlineAreas.Count; index++)
                {
                    var corner = _searchTemplate.CornerOutlineAreas[index];
                    _bitmapUtils.GetArea(
                            grayImage,
                            new Rectangle(
                                x + corner.Width.From,
                                topOutline + corner.Height.From,
                                (x + corner.Width.To) - (x + corner.Width.From),
                                (topOutline + corner.Height.To) - (topOutline + corner.Height.From)))
                        .Save("Debug/Corner_" + index + ".png");
                }

                if (_searchTemplate.CornerOutlineAreas.All(cornerArea => IsAreaInOutlineRange(image,
                        x + cornerArea.Width.From,
                        topOutline + cornerArea.Height.From,
                        x + cornerArea.Width.To,
                        topOutline + cornerArea.Height.To,
                        cornerArea.Threshold)) == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Icon
                _bitmapUtils.GetArea(grayImage,
                        new Rectangle(
                            x + _searchTemplate.IconArea.Width.From,
                            topOutline + _searchTemplate.IconArea.Height.From,
                            (x + _searchTemplate.IconArea.Width.To) - (x + _searchTemplate.IconArea.Width.From),
                            (topOutline + _searchTemplate.IconArea.Height.To) - (topOutline + _searchTemplate.IconArea.Height.From)))
                    .Save("Debug/Icon.png");

                _bitmapUtils.GetArea(grayImage,
                        new Rectangle(
                            x + _searchTemplate.EmptyCenterArea.Width.From,
                            topOutline + _searchTemplate.EmptyCenterArea.Height.From,
                            (x + _searchTemplate.EmptyCenterArea.Width.To) - (x + _searchTemplate.EmptyCenterArea.Width.From),
                            (topOutline + _searchTemplate.EmptyCenterArea.Height.To) - (topOutline + _searchTemplate.EmptyCenterArea.Height.From)))
                    .Save("Debug/EmptyCenter.png");

                if ((IsAreaBrighterThenOutline(grayImage,
                         x + _searchTemplate.IconArea.Width.From,
                         topOutline + _searchTemplate.IconArea.Height.From,
                         x + _searchTemplate.IconArea.Width.To,
                         topOutline + _searchTemplate.IconArea.Height.To,
                         _searchTemplate.IconAreaThreshold) == false ||
                     IsAreaInOutlineRange(grayImage,
                         x + _searchTemplate.EmptyCenterArea.Width.From,
                         topOutline + _searchTemplate.EmptyCenterArea.Height.From,
                         x + _searchTemplate.EmptyCenterArea.Width.To,
                         topOutline + _searchTemplate.EmptyCenterArea.Height.To,
                         _searchTemplate.EmptyCenterAreaThreshold))
                    &&
                    IsIconAreaWithinColorRange(image,
                        _searchTemplate.IconColorRanges,
                        x + _searchTemplate.IconArea.Width.From,
                        topOutline + _searchTemplate.IconArea.Height.From,
                        x + _searchTemplate.IconArea.Width.To,
                        topOutline + _searchTemplate.IconArea.Height.To,
                        _searchTemplate.IconAreaThreshold) == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Result
                var area = new Rectangle(
                    _searchTemplate.TemplateSearchArea.Width.From + leftOutline - _searchTemplate.BackgroundPadding,
                    _searchTemplate.TemplateSearchArea.Height.From + topOutline - _searchTemplate.BackgroundPadding,
                    _searchTemplate.DialogOptionWidth,
                    bottomOutline - topOutline + _searchTemplate.BackgroundPadding * 2);
                dialogOptionsList.Add(area);

                var debugArea = new Rectangle(
                    leftOutline - _searchTemplate.BackgroundPadding,
                    topOutline - _searchTemplate.BackgroundPadding,
                    _searchTemplate.TemplateSearchArea.Width.Length,
                    bottomOutline - topOutline + _searchTemplate.BackgroundPadding * 2);
                dialogOptionsDebugList.Add(debugArea);
                #endregion

                y = area.Bottom + _searchTemplate.Gap - 1;
                isFoundByY = true;
            }

            x = isFoundByY 
                ? x + _searchTemplate.TemplateSearchArea.Width.Length - 1 
                : x + _searchTemplate.VerticalOutlineSearchRangeX.Length - 1;
        }

        _bitmapUtils.DrawRectangles(grayImage, dialogOptionsDebugList);
        grayImage.Save("Debug/Result.png");

        return dialogOptionsList;
    }

    #region Utils
    private (bool, int) GetTopOutline(Bitmap image, int x, int y, int threshold)
    {
        var maxSearchPoint = y + _searchTemplate.TopOutlineSearchRangeY.To;
        if (maxSearchPoint > image.Height - 1) return (false, maxSearchPoint);

        var firstOutline = GetYOutline(image,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
            y + _searchTemplate.TopOutlineSearchRangeY.From,
            maxSearchPoint, 
            threshold);
        if (firstOutline == -1) return (false, y + _searchTemplate.TopOutlineSearchRangeY.To);

        var nextOrdinal = GetYDarkerThenOutline(image,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
            firstOutline + 1,
            maxSearchPoint, 
            threshold);
        if (nextOrdinal == -1) return (true, maxSearchPoint);
        
        var nextOutline = GetYOutline(image,
                x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
                x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
                nextOrdinal,
                maxSearchPoint, 
                threshold);

        return nextOutline == -1 ? (true, firstOutline) : (false, nextOutline - 1);
    }

    private (bool, int) GetBottomOutline(Bitmap image, int x, int y, int threshold)
    {
        var maxSearchPoint = y + _searchTemplate.BottomOutlineSearchRangeY.To;
        if (maxSearchPoint > image.Height - 1) return (false, maxSearchPoint);

        var firstOutlinePosition = GetYOutline(image,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
            x +_searchTemplate.HorizontalOutlineSearchRangeX.To,
            y + _searchTemplate.BottomOutlineSearchRangeY.From,
            maxSearchPoint,
            threshold);

        if (firstOutlinePosition == -1) return (false, y);

        var ordinalLinePosition = GetYOrdinal(image,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.From,
            x + _searchTemplate.HorizontalOutlineSearchRangeX.To,
            firstOutlinePosition + 1,
            maxSearchPoint, 
            threshold);

        return (true, ordinalLinePosition == -1 ? maxSearchPoint : ordinalLinePosition - 1);
    }

    private int GetYOutline(Bitmap image, int x, int maxX, int y, int maxY, int threshold)
    {
        for (var i = y; i <= maxY; i++)
        {
            if (IsHorizontalOutline(image, x, i, maxX, threshold)) return i;
        }
        return -1;
    }

    private int GetYOrdinal(Bitmap image, int x, int maxX, int y, int maxY, int threshold)
    {
        for (var i = y; i <= maxY; i++)
        {
            if (IsHorizontalOutline(image, x, i, maxX, threshold) == false) return i;
        }
        return -1;
    }

    private int GetYDarkerThenOutline(Bitmap image, int x, int maxX, int y, int maxY, int threshold)
    {
        for (var i = y; i <= maxY; i++)
        {
            if (IsDarkerThenOutline(image, x, i, maxX, threshold)) return i;
        }
        return -1;
    }

    private bool IsHorizontalOutline(Bitmap image, int x, int y, int maxX, int threshold)
    {
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            if (_bitmapUtils.IsWithinChannel(image, _searchTemplate.OutlineGrayChannelRange, i, y)) resultPoints++;
        }

        return resultPoints >= threshold;
    }

    private bool IsDarkerThenOutline(Bitmap image, int x, int y, int maxX, int threshold)
    {
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            if (_bitmapUtils.IsDarkerThenChannel(image, i, y, _searchTemplate.OutlineGrayChannelRange)) resultPoints++;
        }

        return resultPoints >= threshold;
    }

    private int GetXOutline(Bitmap image, int x, int y, int maxX, int maxY, int threshold)
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

    private int GetXOrdinalBackward(Bitmap image, int x, int maxX, int y, int maxY, int threshold)
    {
        for (var i = maxX; i >= x; i--)
        {
            if (IsVerticalOutline(image, i, y, maxY, threshold) == false) return i;
        }

        return -1;
    }

    private bool IsVerticalOutline(Bitmap image, int x, int y, int maxY, int threshold)
    {
        var resultPoints = 0;

        for (var i = y; i < maxY; i++)
        {
            if (_bitmapUtils.IsWithinChannel(image, _searchTemplate.OutlineGrayChannelRange, x, i)) resultPoints++;
        }

        return resultPoints >= threshold;
    }

    private bool IsAreaBrighterThenOutline(Bitmap image, int x, int y, int maxX, int maxY, int threshold)
    {
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            for (var j = y; j < maxY; j++)
            {
                if (_bitmapUtils.IsBrighterThenChannel(image, _searchTemplate.OutlineGrayChannelRange, i, j)) resultPoints++;
            }
        }

        return resultPoints >= threshold;
    }

    private bool IsIconAreaWithinColorRange(Bitmap image, IEnumerable<ColorRange<Rgba>> ranges, int x, int y, int maxX, int maxY, int threshold)
    {
        return ranges.FirstOrDefault(range => _bitmapUtils.CountInRange(image, range, x, y, maxX, maxY) >= threshold) is not null;
    }

    private bool IsAreaInOutlineRange(Bitmap image, int x, int y, int maxX, int maxY, int threshold)
    {
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            for (var j = y; j < maxY; j++)
            {
                if (_bitmapUtils.IsWithinChannel(image, _searchTemplate.OutlineGrayChannelRange, i, j)) resultPoints++;
            }
        }

        return resultPoints >= threshold;
    }
    #endregion
}
