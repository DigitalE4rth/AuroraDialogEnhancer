using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancerExtensions.Dimensions;
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

    public void DrawLine(Image bitmap, Point from, Point to)
    {
        using var graphics = Graphics.FromImage(bitmap);
        using var pen = new Pen(Color.LimeGreen, 1);
        graphics.DrawLine(pen, from, to);
    }

    public List<Rectangle> GetDialogOptions(Bitmap image)
    {
        var dialogOptionsList = new List<Rectangle>();
        /*using var croppedArea = _bitmapUtils.GetArea(
            image, 
            new Rectangle(
                _searchTemplate.DialogOptionsSearchArea.Width.From,
                _searchTemplate.DialogOptionsSearchArea.Height.From,
                _searchTemplate.DialogOptionsSearchArea.Width.Length,
                _searchTemplate.DialogOptionsSearchArea.Height.Length));*/

        /*using var croppedArea = _bitmapUtils.GetArea(
            rawImage,
            new Rectangle(
                0,
                0,
                rawImage.Width,
                rawImage.Height));*/

        using var croppedImage = _bitmapUtils.ToGrayScale(image);


        /*DrawLine(croppedImage, 
            new Point(_searchTemplate.UpperLeftCorner.Width.From, 0),
            new Point(_searchTemplate.UpperLeftCorner.Width.From, image.Height));

        DrawLine(croppedImage,
            new Point(_searchTemplate.UpperLeftCorner.Width.To, 0),
            new Point(_searchTemplate.UpperLeftCorner.Width.To, image.Height));*/

        //croppedImage.Save("01_Cut.png");
        //croppedImage.Save($"D:\\Dev\\Projects\\E4rth_\\hoyo-dialog-enhancer-resources\\Raw Resolutions\\test\\{image.Width}x{image.Height}x{Guid.NewGuid()}.png");

        
        for (var x = 0; x + _searchTemplate.TemplateSearchArea.Width.Length <= croppedImage.Width; x++)
        {
            var isFound = false;
            for (var y = 0; y < croppedImage.Height; y++)
            {
                //if (y >= 680)
                //{
                    //System.Diagnostics.Debug.WriteLine("a");
                //}

                #region Top outline
                var (isTopFound, topOutline) = GetTopOutline(croppedImage, x, y, _searchTemplate.HorizontalOutlineThreshold);
                if (isTopFound == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Bottom outline
                var (isBottomFound, bottomOutline) = GetBottomOutline(croppedImage, x, topOutline, _searchTemplate.HorizontalOutlineThreshold);
                if (isBottomFound == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Has icon
                /*using var croppedIcon = _bitmapUtils.GetArea(
                    croppedImage,
                    new Rectangle(
                        x + _searchTemplate.IconArea.Width.From,
                        topOutline + _searchTemplate.IconArea.Height.From,
                        (x + _searchTemplate.IconArea.Width.To) - (x + _searchTemplate.IconArea.Width.From),
                        (topOutline + _searchTemplate.IconArea.Height.To) - (topOutline + _searchTemplate.IconArea.Height.From)));
                croppedIcon.Save("02_Icon.png");*/

                if (IsAreaBrighterThenOutline(croppedImage,
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

                #region Empty center
                /*using var croppedArea = _bitmapUtils.GetArea(
                    croppedImage,
                    new Rectangle(
                        x + _searchTemplate.EmptyCenterArea.Width.From,
                        topOutline + _searchTemplate.EmptyCenterArea.Height.From,
                        (x + _searchTemplate.EmptyCenterArea.Width.To) - (x + _searchTemplate.EmptyCenterArea.Width.From),
                        (topOutline + _searchTemplate.EmptyCenterArea.Height.To) - (topOutline + _searchTemplate.EmptyCenterArea.Height.From)));
                croppedArea.Save("03_EmptyCenter.png");*/

                if (IsAreaInOutlineRange(croppedImage,
                        x + _searchTemplate.EmptyCenterArea.Width.From,
                        topOutline + _searchTemplate.EmptyCenterArea.Height.From,
                        x + _searchTemplate.EmptyCenterArea.Width.To,
                        topOutline + _searchTemplate.EmptyCenterArea.Height.To,
                        _searchTemplate.EmptyCenterAreaThreshold))
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
                    _searchTemplate.VerticalOutlineThreshold);
                if (leftOutline == -1)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Corners
                /*for (var i = 0; i < _searchTemplate.CornerOutlineAreas.Count; i++)
                {
                    var cornerArea = _searchTemplate.CornerOutlineAreas[i];
                    using var croppedCornerArea = _bitmapUtils.GetArea(
                        croppedImage,
                        new Rectangle(
                            x + cornerArea.Width.From,
                            topOutline + cornerArea.Height.From,
                            (x + cornerArea.Width.To) - (x + cornerArea.Width.From),
                            (topOutline + cornerArea.Height.To) - (topOutline + cornerArea.Height.From)));

                    croppedCornerArea.Save($"04_Corner_{i}.png");
                }*/

                /*if (IsAreasInRange(image, _searchTemplate.CornerOutlineAreas, x, y))
                {
                    y = topOutline;
                    continue;
                }*/

                if (_searchTemplate.CornerOutlineAreas.All(cornerArea => IsAreaHasOutlinePixel(image,
                        cornerArea,
                        x + cornerArea.Width.From,
                        topOutline + cornerArea.Height.From,
                        x + cornerArea.Width.To,
                        topOutline + cornerArea.Height.To)) == false)
                {
                    y = topOutline;
                    continue;
                }
                #endregion

                #region Result wrapping
                var area = new Rectangle(
                    _searchTemplate.TemplateSearchArea.Width.From + leftOutline - _searchTemplate.BackgroundPadding,
                    _searchTemplate.TemplateSearchArea.Height.From + topOutline - _searchTemplate.BackgroundPadding,
                    _searchTemplate.DialogOptionWidth,
                    bottomOutline - topOutline + _searchTemplate.BackgroundPadding * 2);
                dialogOptionsList.Add(area);
                #endregion

                y = area.Bottom + _searchTemplate.Gap;
                isFound = true;
            }

            x = isFound 
                ? x + _searchTemplate.TemplateSearchArea.Width.Length 
                : x + _searchTemplate.VerticalOutlineSearchRangeX.Length - 1;
        }
        
        _bitmapUtils.DrawRectangles(croppedImage, dialogOptionsList);
        //croppedImage.Save("05_Result.png");
        
        return dialogOptionsList;
    }

    #region Utils
    private (bool, int) GetTopOutline(Bitmap image, int x, int y, int threshold)
    {
        var maxSearchPoint = y + _searchTemplate.TopOutlineSearchRangeY.To;
        if (maxSearchPoint >= image.Height) return (false, maxSearchPoint);

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
        if (maxSearchPoint >= image.Height) return (false, maxSearchPoint);

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
            if (_bitmapUtils.IsWithinGrayRange(image, i, y, _searchTemplate.OutlineGrayChannelRange)) resultPoints++;
        }

        return resultPoints >= threshold;
    }

    private bool IsDarkerThenOutline(Bitmap image, int x, int y, int maxX, int threshold)
    {
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            if (_bitmapUtils.IsDarkerGrayRange(image, i, y, _searchTemplate.OutlineGrayChannelRange)) resultPoints++;
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
            if (_bitmapUtils.IsWithinGrayRange(image, x, i, _searchTemplate.OutlineGrayChannelRange)) resultPoints++;
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
                if (_bitmapUtils.IsBrighterGrayRange(image, i, j, _searchTemplate.OutlineGrayChannelRange)) resultPoints++;
            }
        }

        return resultPoints >= threshold;
    }

    private bool IsAreaInOutlineRange(Bitmap image, int x, int y, int maxX, int maxY, int threshold)
    {
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            for (var j = y; j < maxY; j++)
            {
                if (_bitmapUtils.IsWithinGrayRange(image, i, j, _searchTemplate.OutlineGrayChannelRange)) resultPoints++;
            }
        }

        return resultPoints >= threshold;
    }

    private bool IsAreaHasOutlinePixel(Bitmap image, Area area, int x, int y, int maxX, int maxY)
    {
        for (var i = x; i < maxX; i++)
        {
            for (var j = y; j < maxY; j++)
            {
                if (_bitmapUtils.IsWithinGrayRange(image, i, j, _searchTemplate.OutlineGrayChannelRange)) return true;
            }
        }

        return false;
    }

    #endregion
}
