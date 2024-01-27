using System;
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
    private readonly BitmapUtils    _bitmapUtils;
    private readonly SearchTemplate _searchTemplate;
    private int _speakerFirstLineY;
    
    public DialogOptionFinder(SearchTemplate searchTemplate)
    {
        _searchTemplate = searchTemplate;
        _bitmapUtils    = new BitmapUtils();
    }

    public bool IsDialogMode(params Bitmap[] image)
    {
        var isDialogIndication = _searchTemplate.DialogIndicationColorRange.Any(color => _bitmapUtils.CountInRange(image[0], color) > 0);

        var (firstLineY, inRangeCount) = _bitmapUtils.GetFirstLineAndCountInRange(image[1], _searchTemplate.SpeakerColorRange);
        var isSpeakerNamePresent = inRangeCount > _searchTemplate.SpeakerNameThreshold;

        if (!isDialogIndication && !isSpeakerNamePresent) return false;

        _speakerFirstLineY = isSpeakerNamePresent
            ? _searchTemplate.SpeakerNameArea.Height.From + firstLineY
            : image[1].Height;

        return true;
    }

    public List<Rectangle> GetDialogOptions(Bitmap image)
    {
        var dialogOptionsList = new List<Rectangle>();

        for (var y = 0; y <= _speakerFirstLineY - 1; y++)
        {
            #region Detect Color
            var colorRange = _searchTemplate.DialogOptionColorRanges.FirstOrDefault(colorWrapper =>
                _bitmapUtils.CountInRange(
                    image,
                    colorWrapper.IconColor,
                    _searchTemplate.IconHorizontalRange.From,
                    y,
                    _searchTemplate.IconHorizontalRange.To,
                    y) 
                > 0);

            if (colorRange is null) continue;
            #endregion

            #region Detect Icon
            // Top Left
            var (iconTopLeftX, iconTopLeftY) = GetTopLeftIconPosition(image, colorRange.IconColor, y);
            if (iconTopLeftX == -1) continue;

            // Bottom right
            var (iconBottomRightX, iconBottomRightY) = GetBottomRightIconPosition(image, colorRange.IconColor, iconTopLeftY);
            if (iconBottomRightX == -1) continue;

            // Icon between min and max sizes
            var calculatedIconHeight = iconBottomRightY - iconTopLeftY;
            var calculatedIconLength = iconBottomRightX - iconTopLeftX;
            if (calculatedIconHeight > _searchTemplate.IconMaxLength ||
                calculatedIconHeight < _searchTemplate.IconMinLength ||
                calculatedIconLength > _searchTemplate.IconMaxLength ||
                calculatedIconLength < _searchTemplate.IconMinLength)
            {
                continue;
            }

            // Icon area has minimum pixels amount of detected color
            if (_bitmapUtils.CountInRange(
                    image,
                    colorRange.IconColor,
                    iconTopLeftX,
                    iconTopLeftY,
                    iconBottomRightX,
                    iconBottomRightY) 
                < _searchTemplate.IconThreshold * ((iconBottomRightX - iconTopLeftX) * (iconBottomRightY - iconTopLeftY)))
            {
                continue;
            }

            var iconCenter = iconTopLeftY + calculatedIconHeight / 2;
            #endregion

            #region Detect Text
            var textBottomY = GetTextBottomY(image, colorRange.TextColors, iconCenter);
            if (textBottomY == -1) continue;

            var maxTextHeightAbove = dialogOptionsList.Any() ? dialogOptionsList.Last().Bottom : -1;
            var textTopY = GetTextTopY(image, colorRange.TextColors, iconCenter, maxTextHeightAbove);
            if (textTopY == -1) continue;

            if (!IsTopIconAreaClear(image, colorRange, textTopY, iconTopLeftY) ||
                !IsBottomIconAreaClear(image, colorRange, textBottomY, iconBottomRightY))
            {
                continue;
            }

            AdjustTextBoundaries(iconCenter, ref textTopY, ref textBottomY);

            var textAreaHeight = textBottomY - textTopY;
            var textTopBottomMargin = textAreaHeight <= _searchTemplate.TextLineHeight
                ? _searchTemplate.TextSingleTopBottomMargin
                : _searchTemplate.TextMultipleTopBottomMargin;
            #endregion

            #region Result
            dialogOptionsList.Add(new Rectangle(
                _searchTemplate.TemplateSearchArea.Width.From + _searchTemplate.IconHorizontalRange.From,
                _searchTemplate.TemplateSearchArea.Height.From + textTopY - textTopBottomMargin,
                _searchTemplate.DialogOptionWidth,
                (textBottomY + textTopBottomMargin) - (textTopY - textTopBottomMargin)));
            #endregion

            y = textBottomY + textTopBottomMargin + _searchTemplate.Gap - 1;
        }

        return dialogOptionsList;
    }

    #region Icon
    private bool IsBottomIconAreaClear(Bitmap image, ColorWrapper colorRange, int textBottomY, int iconBottomY)
    {
        if (iconBottomY == textBottomY) return true;

        // Icon is bigger
        var maxOffsetY = textBottomY + _searchTemplate.IconClearAreaSearchOffset;
        if (maxOffsetY >= iconBottomY) return true;

        if (iconBottomY < textBottomY && !IsAreaContainsPixelBrighterThenColor(
                image,
                colorRange.IconColor.Low,
                _searchTemplate.IconHorizontalRange.From,
                iconBottomY + 1,
                _searchTemplate.IconHorizontalRange.From + _searchTemplate.IconHorizontalRange.Length - 1,
                maxOffsetY))
        {
            return true;
        }

        // Text is bigger
        maxOffsetY = iconBottomY + _searchTemplate.IconClearAreaSearchOffset;
        if (maxOffsetY >= textBottomY) return true;

        if (iconBottomY > textBottomY && !IsAreaContainsPixelBrighterThenColor(
                image,
                colorRange.IconColor.Low,
                _searchTemplate.TextHorizontalRange.From,
                textBottomY + 1,
                _searchTemplate.TextHorizontalRange.From + _searchTemplate.TextHorizontalRange.Length - 1,
                maxOffsetY))
        {
            return true;
        }

        return false;
    }

    private bool IsTopIconAreaClear(Bitmap image, ColorWrapper colorRange, int textTopY, int iconTopY)
    {
        if (iconTopY == textTopY) return true;

        // Icon is bigger
        var maxOffsetY = textTopY - _searchTemplate.IconClearAreaSearchOffset;
        if (maxOffsetY >= iconTopY) return true;

        if (iconTopY < textTopY && !IsAreaContainsPixelBrighterThenColor(
                image,
                colorRange.IconColor.Low,
                _searchTemplate.TextHorizontalRange.From,
                iconTopY,
                _searchTemplate.TextHorizontalRange.From + _searchTemplate.TextHorizontalRange.Length - 1,
                maxOffsetY))
        {
            return true;
        }

        // Text is bigger
        maxOffsetY = iconTopY - _searchTemplate.IconClearAreaSearchOffset;
        if (maxOffsetY >= textTopY) return true;

        if (iconTopY > textTopY && !IsAreaContainsPixelBrighterThenColor(
                image,
                colorRange.IconColor.Low,
                _searchTemplate.IconHorizontalRange.From,
                iconTopY,
                _searchTemplate.IconHorizontalRange.From + _searchTemplate.IconHorizontalRange.Length - 1,
                textTopY - 1))
        {
            return true;
        }

        return false;
    }

    private (int, int) GetTopLeftIconPosition(Bitmap image, ColorRange colorRange, int y)
    {
        var maxSearchY = y + _searchTemplate.IconMaxLength;
        if (maxSearchY > _speakerFirstLineY - 1) return (-1, -1);

        var leftMostX = GetLeftMost(
            image, colorRange,
            _searchTemplate.IconHorizontalRange.From, y,
            _searchTemplate.IconHorizontalRange.To, maxSearchY);
        if (leftMostX == -1) return (-1, -1);

        var topMostY = GetTopMost(
            image, colorRange,
            _searchTemplate.IconHorizontalRange.From, y,
            _searchTemplate.IconHorizontalRange.To, maxSearchY);
        if (topMostY != -1) return (leftMostX, topMostY);

        return (-1, -1);
    }

    private (int, int) GetBottomRightIconPosition(Bitmap image, ColorRange colorRange, int y)
    {
        var maxSearchY = y + _searchTemplate.IconMaxLength;
        if (maxSearchY > _speakerFirstLineY - 1) return (-1, -1);

        var rightMostX = GetRightMost(
            image, colorRange,
            _searchTemplate.IconHorizontalRange.From, y,
            _searchTemplate.IconHorizontalRange.To, maxSearchY);
        if (rightMostX == -1) return (-1, -1);

        var bottomMostY = GetBottomMost(
            image, colorRange,
            _searchTemplate.IconHorizontalRange.From, y,
            _searchTemplate.IconHorizontalRange.To, maxSearchY);
        if (bottomMostY != -1) return (rightMostX, bottomMostY);

        return (-1, -1);
    }
    #endregion



    #region Text
    private int GetTextBottomY(Bitmap image, ColorRange[] textColors, int iconCenterY)
    {
        var oneStart = iconCenterY - _searchTemplate.TextLineHeightHalf;
        var oneEnd = iconCenterY + _searchTemplate.TextLineHeightHalf;
        var lastLineOfOnePosition = CountTextLinesBelow(image, textColors, oneStart, oneEnd);

        var twoStart = iconCenterY;
        var twoEnd = twoStart + _searchTemplate.TextLineHeight;
        var lastLineOfTwoPosition = CountTextLinesBelow(image, textColors, twoStart, twoEnd);

        var threeStart = iconCenterY + _searchTemplate.TextLineHeightHalf;
        var threeEnd = threeStart + _searchTemplate.TextLineHeight;
        var lastLineOfThreePosition = CountTextLinesBelow(image, textColors, threeStart, threeEnd);

        var fourStart = iconCenterY + _searchTemplate.TextLineHeight;
        var fourEnd = fourStart + _searchTemplate.TextLineHeight;
        var lastLineOfFourPosition = CountTextLinesBelow(image, textColors, fourStart, fourEnd);

        if (lastLineOfOnePosition   == -1) return -1;
        if (lastLineOfTwoPosition   == -1 || lastLineOfTwoPosition   == lastLineOfOnePosition)   return lastLineOfOnePosition;
        if (lastLineOfThreePosition == -1 || lastLineOfThreePosition == lastLineOfTwoPosition)   return lastLineOfTwoPosition;
        if (lastLineOfFourPosition  == -1 || lastLineOfFourPosition  == lastLineOfThreePosition) return lastLineOfThreePosition;
        return lastLineOfFourPosition;
    }

    private int GetTextTopY(Bitmap image, ColorRange[] textColors, int iconCenterY, int maxAbove)
    {
        var oneStart = iconCenterY + _searchTemplate.TextLineHeightHalf;
        var oneEnd = iconCenterY - _searchTemplate.TextLineHeightHalf;
        var firstLineOfOnePosition = GetTopMostAboveTextLine(image, textColors, oneStart, oneEnd);

        var twoStart = iconCenterY;
        var twoEnd = maxAbove == -1 ? twoStart - _searchTemplate.TextLineHeight : maxAbove;
        var firstLineOfTwoPosition = GetTopMostAboveTextLine(image, textColors, twoStart, twoEnd);

        var threeStart = iconCenterY - _searchTemplate.TextLineHeightHalf;
        var threeEnd = maxAbove == -1 ? threeStart - _searchTemplate.TextLineHeight : maxAbove;
        var firstLineOfThreePosition = GetTopMostAboveTextLine(image, textColors, threeStart, threeEnd);

        var fourStart = iconCenterY - _searchTemplate.TextLineHeight;
        var fourEnd = maxAbove == -1 ? fourStart - _searchTemplate.TextLineHeight : maxAbove;
        var firstLineOfFourPosition = GetTopMostAboveTextLine(image, textColors, fourStart, fourEnd);

        if (firstLineOfOnePosition   == -1) return -1;
        if (firstLineOfTwoPosition   == -1 || firstLineOfTwoPosition   == firstLineOfOnePosition)   return firstLineOfOnePosition;
        if (firstLineOfThreePosition == -1 || firstLineOfThreePosition == firstLineOfTwoPosition)   return firstLineOfTwoPosition;
        if (firstLineOfFourPosition  == -1 || firstLineOfFourPosition  == firstLineOfThreePosition) return firstLineOfThreePosition;
        return firstLineOfFourPosition;
    }

    private int CountTextLinesBelow(Bitmap image, IEnumerable<ColorRange> textColors, int startY, int maxBelow)
    {
        var calculatedMaxAbove = startY < 0 ? 0 : startY;
        var calculatedMaxBelow = maxBelow > _speakerFirstLineY - 1 ? _speakerFirstLineY - 1 : maxBelow;

        return textColors
            .Select(color => GetLineLast(image, color,
                _searchTemplate.TextHorizontalRange.From,
                calculatedMaxAbove,
                _searchTemplate.TextHorizontalRange.From + _searchTemplate.TextHorizontalRange.Length - 1,
                calculatedMaxBelow))
            .OrderBy(i => i)
            .Last();
    }

    private int GetTopMostAboveTextLine(Bitmap image, IEnumerable<ColorRange> textColors, int startY, int maxAbove)
    {
        var calculatedMaxAbove = maxAbove < 0 ? 0 : maxAbove;

        return textColors
            .Select(color => GetLineLastReverse(image, color,
                _searchTemplate.TextHorizontalRange.From,
                startY,
                _searchTemplate.TextHorizontalRange.From + _searchTemplate.TextHorizontalRange.Length - 1,
                calculatedMaxAbove))
            .OrderBy(i => i)
            .First();
    }

    private void AdjustTextBoundaries(int iconCenter, ref int topTextY, ref int bottomTextY)
    {
        var topTextHeight = iconCenter - topTextY;
        var bottomTextHeight = bottomTextY - iconCenter;
        if (Math.Abs(topTextHeight - bottomTextHeight) <= _searchTemplate.TextLineHeightHalf) return;

        if (topTextHeight > bottomTextHeight)
        {
            topTextY = iconCenter - bottomTextHeight;
            return;
        }

        bottomTextY = iconCenter + topTextHeight;
    }
    #endregion



    #region Utils
    public int GetTopMost(Bitmap image, ColorRange colorRange, int x, int y, int maxX, int maxY)
    {
        for (var y0 = y; y0 <= maxY; y0++)
        {
            for (var x0 = x; x0 <= maxX; x0++)
            {
                if (_bitmapUtils.IsWithinRange(image, colorRange, x0, y0)) return y0;
            }
        }

        return -1;
    }

    public int GetLeftMost(Bitmap image, ColorRange colorRange, int x, int y, int maxX, int maxY)
    {
        for (var x0 = x; x0 <= maxX; x0++) 
        {
            for (var y0 = y; y0 <= maxY; y0++)
            {
                if (_bitmapUtils.IsWithinRange(image, colorRange, x0, y0)) return x0;
            }
        }

        return -1;
    }

    public int GetRightMost(Bitmap image, ColorRange colorRange, int x, int y, int maxX, int maxY)
    {
        for (var x0 = maxX; x <= x0; x0--)
        {
            for (var y0 = maxY; y <= y0; y0--)
            {
                if (_bitmapUtils.IsWithinRange(image, colorRange, x0, y0)) return x0;
            }
        }

        return -1;
    }

    public int GetBottomMost(Bitmap image, ColorRange colorRange, int x, int y, int maxX, int maxY)
    {
        for (var y0 = maxY; y <= y0; y0--)
        {
            for (var x0 = maxX; x <= x0; x0--)
            {
                if (_bitmapUtils.IsWithinRange(image, colorRange, x0, y0)) return y0;
            }
        }

        return -1;
    }

    private bool IsAreaContainsPixelBrighterThenColor(Bitmap image, Rgba color, int x, int y, int maxX, int maxY)
    {
        for (var yo = y; yo <= maxY; yo++)
        {
            for (var xo = x; xo <= maxX; xo++)
            {
                if (!_bitmapUtils.IsDarkerThenColor(image, color, xo, yo)) 
                    return true;
            }
        }

        return false;
    }

    private int GetLineLast(Bitmap image, ColorRange colorRange, int x, int y, int maxX, int maxY)
    {
        for (var y0 = y; y0 <= maxY; y0++)
        {
            for (var x0 = x; x0 < maxX; x0++)
            {
                if (_bitmapUtils.IsWithinRange(image, colorRange, x0, y0)) return y0;
            }
        }

        return -1;
    }

    private int GetLineLastReverse(Bitmap image, ColorRange colorRange, int x, int y, int maxX, int maxY)
    {
        for (var y0 = y; y0 >= maxY; y0--)
        {
            for (var x0 = x; x0 < maxX; x0++)
            {
                if (_bitmapUtils.IsWithinRange(image, colorRange, x0, y0)) return y0;
            }
        }

        return -1;
    }
    #endregion
}
