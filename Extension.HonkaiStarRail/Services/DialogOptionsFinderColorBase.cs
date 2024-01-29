using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Services;
using Extension.HonkaiStarRail.Templates;
using Extension.HonkaiStarRail.Utils;

namespace Extension.HonkaiStarRail.Services;

internal abstract class DialogOptionsFinderBase<T> where T : IColor 
{
    protected readonly BitmapUtils BitmapUtils = new();
    protected SearchTemplate SearchTemplate;

    protected DialogOptionsFinderBase(SearchTemplate searchTemplate)
    {
        SearchTemplate = searchTemplate;
    }

    public void FindAndAddDialogOptionRectangle(
        Bitmap image,
        ColorWrapper<T> colorWrapper,
        ref int y,
        int maxY,
        ICollection<Rectangle> dialogOptions,
        ICollection<Rectangle> dialogOptionsDebug)
    {
        #region Detect Icon
        // Top Left
        var (iconTopLeftX, iconTopLeftY) = GetTopLeftIconPosition(image, colorWrapper.IconColor, y, maxY);
        if (iconTopLeftX == -1) return;

        // Bottom right
        var (iconBottomRightX, iconBottomRightY) = GetBottomRightIconPosition(image, colorWrapper.IconColor, iconTopLeftY, maxY);
        if (iconBottomRightX == -1) return;

        // Icon between min and max sizes
        var calculatedIconHeight = iconBottomRightY - iconTopLeftY;
        var calculatedIconLength = iconBottomRightX - iconTopLeftX;
        if (calculatedIconHeight > SearchTemplate.IconMaxLength ||
            calculatedIconHeight < SearchTemplate.IconMinLength ||
            calculatedIconLength > SearchTemplate.IconMaxLength ||
            calculatedIconLength < SearchTemplate.IconMinLength)
        {
            return;
        }

        // Icon area has minimum pixels amount of detected color
        var iconAreaInRangeCount = CountInRange(image, colorWrapper.IconColor, iconTopLeftX, iconTopLeftY, iconBottomRightX, iconBottomRightY);
        if (iconAreaInRangeCount < SearchTemplate.IconThreshold * ((iconBottomRightX - iconTopLeftX) * (iconBottomRightY - iconTopLeftY)))
        {
            return;
        }

        var iconCenter = iconTopLeftY + calculatedIconHeight / 2;
        #endregion

        #region Detect Text
        var textBottomY = GetTextBottomY(image, colorWrapper.TextColors, iconCenter, maxY);
        if (textBottomY == -1) return;

        var maxTextHeightAbove = dialogOptions.Any() ? dialogOptions.Last().Bottom : -1;
        var textTopY = GetTextTopY(image, colorWrapper.TextColors, iconCenter, maxTextHeightAbove);
        if (textTopY == -1) return;

        var offset = (int)((iconBottomRightY - iconTopLeftY) * SearchTemplate.IconClearAreaSearchOffset);
        if (!IsTopIconAreaClear(image, colorWrapper, textTopY, iconTopLeftY, offset) ||
            !IsBottomIconAreaClear(image, colorWrapper, textBottomY, iconBottomRightY, offset))
        {
            return;
        }

        AdjustTextBoundaries(iconCenter, ref textTopY, ref textBottomY);

        var textAreaHeight = textBottomY - textTopY;
        var textTopBottomMargin = textAreaHeight <= SearchTemplate.TextLineHeight
            ? SearchTemplate.TextSingleTopBottomMargin
            : SearchTemplate.TextMultipleTopBottomMargin;
        #endregion

        #region Result
        // Debug
        dialogOptionsDebug.Add(new Rectangle(
            SearchTemplate.IconHorizontalRange.From,
            textTopY - textTopBottomMargin,
            image.Width - SearchTemplate.IconHorizontalRange.From - 1,
            (textBottomY + textTopBottomMargin) - (textTopY - textTopBottomMargin)));

        dialogOptions.Add(new Rectangle(
            SearchTemplate.TemplateSearchArea.Width.From + SearchTemplate.IconHorizontalRange.From,
            SearchTemplate.TemplateSearchArea.Height.From + textTopY - textTopBottomMargin,
            SearchTemplate.DialogOptionWidth,
            (textBottomY + textTopBottomMargin) - (textTopY - textTopBottomMargin)));
        #endregion

        y = textBottomY + textTopBottomMargin + SearchTemplate.Gap - 1;
    }


    #region Bimap factory
    protected virtual int CountInRange(Bitmap image, ColorRange<T> colorRange, int x, int y, int maxX, int maxY)
    {
        throw new NotImplementedException();
    }

    protected virtual bool IsWithinRange(Bitmap image, ColorRange<T> colorRange, int x, int y)
    {
        throw new NotImplementedException();
    }

    protected virtual bool IsDarkerThenColor(Bitmap image, T color, int x, int y)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Icon
    protected bool IsBottomIconAreaClear(Bitmap image, ColorWrapper<T> colorRange, int textBottomY, int iconBottomY, int offset)
    {
        if (iconBottomY == textBottomY) return true;

        // Icon is bigger
        var maxOffsetY = textBottomY + offset;
        if (maxOffsetY >= iconBottomY) return true;

        if (iconBottomY < textBottomY && !IsAreaContainsPixelBrighterThenColor(
                image,
                colorRange.IconColor.Low,
                SearchTemplate.IconHorizontalRange.From,
                iconBottomY + 1,
                SearchTemplate.IconHorizontalRange.To - 1,
                maxOffsetY))
        {
            return true;
        }

        // Text is bigger
        maxOffsetY = iconBottomY + offset;
        if (maxOffsetY >= textBottomY) return true;

        return iconBottomY > textBottomY && !IsAreaContainsPixelBrighterThenColor(
            image,
            colorRange.IconColor.Low,
            SearchTemplate.TextHorizontalRange.From,
            textBottomY + 1,
            SearchTemplate.TextHorizontalRange.To - 1,
            maxOffsetY);
    }

    protected bool IsTopIconAreaClear(Bitmap image, ColorWrapper<T> colorRange, int textTopY, int iconTopY, int offset)
    {
        if (iconTopY == textTopY) return true;

        // Icon is bigger
        var maxOffsetY = textTopY - offset;
        if (maxOffsetY >= iconTopY) return true;

        if (iconTopY < textTopY && !IsAreaContainsPixelBrighterThenColor(
                image,
                colorRange.IconColor.Low,
                SearchTemplate.TextHorizontalRange.From,
                iconTopY,
                SearchTemplate.TextHorizontalRange.To - 1,
                maxOffsetY))
        {
            return true;
        }

        // Text is bigger
        maxOffsetY = iconTopY - offset;
        if (maxOffsetY >= textTopY) return true;

        return iconTopY > textTopY && !IsAreaContainsPixelBrighterThenColor(
            image,
            colorRange.IconColor.Low,
            SearchTemplate.IconHorizontalRange.From,
            iconTopY,
            SearchTemplate.IconHorizontalRange.To - 1,
            textTopY - 1);
    }

    protected (int, int) GetTopLeftIconPosition(Bitmap image, ColorRange<T> colorRangeRgb, int y, int maxY)
    {
        var maxSearchY = y + SearchTemplate.IconMaxLength;
        if (maxSearchY > maxY - 1) return (-1, -1);

        var leftMostX = GetLeftMost(
            image, colorRangeRgb,
            SearchTemplate.IconHorizontalRange.From, y,
            SearchTemplate.IconHorizontalRange.To, maxSearchY);
        if (leftMostX == -1) return (-1, -1);

        var topMostY = GetTopMost(
            image, colorRangeRgb,
            SearchTemplate.IconHorizontalRange.From, y,
            SearchTemplate.IconHorizontalRange.To, maxSearchY);
        return topMostY != -1 ? (leftMostX, topMostY) : (-1, -1);
    }

    protected (int, int) GetBottomRightIconPosition(Bitmap image, ColorRange<T> colorRangeRgb, int y, int maxY)
    {
        var maxSearchY = y + SearchTemplate.IconMaxLength;
        if (maxSearchY > maxY - 1) return (-1, -1);

        var rightMostX = GetRightMost(
            image, colorRangeRgb,
            SearchTemplate.IconHorizontalRange.From, y,
            SearchTemplate.IconHorizontalRange.To, maxSearchY);
        if (rightMostX == -1) return (-1, -1);

        var bottomMostY = GetBottomMost(
            image, colorRangeRgb,
            SearchTemplate.IconHorizontalRange.From, y,
            SearchTemplate.IconHorizontalRange.To, maxSearchY);
        return bottomMostY != -1 ? (rightMostX, bottomMostY) : (-1, -1);
    }
    #endregion



    #region Text
    protected int GetTextBottomY(Bitmap image, ColorRange<T>[] textColors, int iconCenterY, int maxY)
    {
        var oneEnd = iconCenterY + SearchTemplate.TextLineHeightHalf;
        var lastLineOfOnePosition = CountTextLinesBelow(image, textColors, iconCenterY, oneEnd, maxY);

        var twoEnd = iconCenterY + SearchTemplate.TextLineHeight;
        var lastLineOfTwoPosition = CountTextLinesBelow(image, textColors, iconCenterY, twoEnd, maxY);

        var threeStart = iconCenterY + SearchTemplate.TextLineHeightHalf;
        var threeEnd = threeStart + SearchTemplate.TextLineHeight;
        var lastLineOfThreePosition = CountTextLinesBelow(image, textColors, threeStart, threeEnd, maxY);

        var fourStart = iconCenterY + SearchTemplate.TextLineHeight;
        var fourEnd = fourStart + SearchTemplate.TextLineHeight;
        var lastLineOfFourPosition = CountTextLinesBelow(image, textColors, fourStart, fourEnd, maxY);

        if (lastLineOfOnePosition   == -1) return -1;
        if (lastLineOfTwoPosition   == -1 || lastLineOfTwoPosition   == lastLineOfOnePosition)   return lastLineOfOnePosition;
        if (lastLineOfThreePosition == -1 || lastLineOfThreePosition == lastLineOfTwoPosition)   return lastLineOfTwoPosition;
        if (lastLineOfFourPosition  == -1 || lastLineOfFourPosition  == lastLineOfThreePosition) return lastLineOfThreePosition;
        return lastLineOfFourPosition;
    }

    protected int GetTextTopY(Bitmap image, ColorRange<T>[] textColors, int iconCenterY, int maxAbove)
    {
        var oneEnd = iconCenterY - SearchTemplate.TextLineHeightHalf;
        var firstLineOfOnePosition = GetTopMostAboveTextLine(image, textColors, iconCenterY, oneEnd);

        var twoEnd = maxAbove == -1 ? iconCenterY - SearchTemplate.TextLineHeight : maxAbove;
        var firstLineOfTwoPosition = GetTopMostAboveTextLine(image, textColors, iconCenterY, twoEnd);

        var threeStart = iconCenterY - SearchTemplate.TextLineHeightHalf;
        var threeEnd = maxAbove == -1 ? threeStart - SearchTemplate.TextLineHeight : maxAbove;
        var firstLineOfThreePosition = GetTopMostAboveTextLine(image, textColors, threeStart, threeEnd);

        var fourStart = iconCenterY - SearchTemplate.TextLineHeight;
        var fourEnd = maxAbove == -1 ? fourStart - SearchTemplate.TextLineHeight : maxAbove;
        var firstLineOfFourPosition = GetTopMostAboveTextLine(image, textColors, fourStart, fourEnd);

        if (firstLineOfOnePosition   == -1) return -1;
        if (firstLineOfTwoPosition   == -1 || firstLineOfTwoPosition   == firstLineOfOnePosition)    return firstLineOfOnePosition;
        if (firstLineOfThreePosition == -1 || firstLineOfThreePosition == firstLineOfTwoPosition)    return firstLineOfTwoPosition;
        if (firstLineOfFourPosition  == -1 || firstLineOfFourPosition   == firstLineOfThreePosition) return firstLineOfThreePosition;
        return firstLineOfFourPosition;
    }

    protected int CountTextLinesBelow(Bitmap image, IEnumerable<ColorRange<T>> textColors, int startY, int maxBelow, int maxY)
    {
        var calculatedMaxAbove = startY < 0 ? 0 : startY;
        var calculatedMaxBelow = maxBelow > maxY - 1 ? maxY - 1 : maxBelow;

        return textColors.Max(color => GetLineLastReverse(
            image,
            color,
            SearchTemplate.TextHorizontalRange.From,
            calculatedMaxBelow,
            SearchTemplate.TextHorizontalRange.To - 1,
            calculatedMaxAbove));
    }

    protected int GetTopMostAboveTextLine(Bitmap image, IEnumerable<ColorRange<T>> textColors, int startY, int maxAbove)
    {
        var calculatedMaxAbove = maxAbove < 0 ? 0 : maxAbove;

        return textColors.Min(color => GetLineLast(
            image,
            color,
            SearchTemplate.TextHorizontalRange.From,
            calculatedMaxAbove,
            SearchTemplate.TextHorizontalRange.To - 1,
            startY));
    }

    protected void AdjustTextBoundaries(int iconCenter, ref int topTextY, ref int bottomTextY)
    {
        var topTextHeight = iconCenter - topTextY;
        var bottomTextHeight = bottomTextY - iconCenter;
        if (Math.Abs(topTextHeight - bottomTextHeight) <= SearchTemplate.TextLineHeightHalf) return;

        if (topTextHeight > bottomTextHeight)
        {
            topTextY = iconCenter - bottomTextHeight;
            return;
        }

        bottomTextY = iconCenter + topTextHeight;
    }
    #endregion



    #region Utils
    protected int GetTopMost(Bitmap image, ColorRange<T> colorRange, int x, int y, int maxX, int maxY)
    {
        for (var y0 = y; y0 <= maxY; y0++)
        {
            for (var x0 = x; x0 <= maxX; x0++)
            {
                if (IsWithinRange(image, colorRange, x0, y0)) return y0;
            }
        }

        return -1;
    }

    protected int GetLeftMost(Bitmap image, ColorRange<T> colorRange, int x, int y, int maxX, int maxY)
    {
        for (var x0 = x; x0 <= maxX; x0++)
        {
            for (var y0 = y; y0 <= maxY; y0++)
            {
                if (IsWithinRange(image, colorRange, x0, y0)) return x0;
            }
        }

        return -1;
    }

    protected int GetRightMost(Bitmap image, ColorRange<T> colorRange, int x, int y, int maxX, int maxY)
    {
        for (var x0 = maxX; x <= x0; x0--)
        {
            for (var y0 = maxY; y <= y0; y0--)
            {
                if (IsWithinRange(image, colorRange, x0, y0)) return x0;
            }
        }

        return -1;
    }
    protected int GetBottomMost(Bitmap image, ColorRange<T> colorRange, int x, int y, int maxX, int maxY)
    {
        for (var y0 = maxY; y <= y0; y0--)
        {
            for (var x0 = maxX; x <= x0; x0--)
            {
                if (IsWithinRange(image, colorRange, x0, y0)) return y0;
            }
        }

        return -1;
    }

    protected bool IsAreaContainsPixelBrighterThenColor(Bitmap image, T color, int x, int y, int maxX, int maxY)
    {
        for (var yo = y; yo <= maxY; yo++)
        {
            for (var xo = x; xo <= maxX; xo++)
            {
                if (!IsDarkerThenColor(image, color, xo, yo))
                    return true;
            }
        }

        return false;
    }

    protected int GetLineLast(Bitmap image, ColorRange<T> colorRange, int x, int y, int maxX, int maxY)
    {
        for (var y0 = y; y0 <= maxY; y0++)
        {
            for (var x0 = x; x0 < maxX; x0++)
            {
                if (IsWithinRange(image, colorRange, x0, y0)) return y0;
            }
        }

        return -1;
    }

    protected int GetLineLastReverse(Bitmap image, ColorRange<T> colorRange, int x, int y, int maxX, int maxY)
    {
        for (var y0 = y; y0 >= maxY; y0--)
        {
            for (var x0 = x; x0 < maxX; x0++)
            {
                if (IsWithinRange(image, colorRange, x0, y0)) return y0;
            }
        }

        return -1;
    }
    #endregion
}
