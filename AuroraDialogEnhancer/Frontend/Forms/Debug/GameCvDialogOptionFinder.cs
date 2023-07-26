using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AuroraDialogEnhancer.Frontend.Forms.Debug.CvData;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

internal class GameCvDialogOptionFinder
{
    private readonly BitmapUtils            _bitmapUtils;
    private readonly DynamicTemplateService _templateService;
    private SearchTemplate                  _searchTemplate;

    public GameCvDialogOptionFinder()
    {
        _bitmapUtils     = new BitmapUtils();
        _templateService = new DynamicTemplateService();
        _searchTemplate  = new SearchTemplate();
    }

    public void Initialize(Size clientSize)
    {
        _searchTemplate = _templateService.GetDynamicTemplate(clientSize);
    }

    public List<Rectangle> GetDialogOptions(Bitmap rawImage, double threshold = 1)
    {
        var dialogOptionsList = new List<Rectangle>();
        using var croppedArea = _bitmapUtils.GetArea(rawImage, 
            new Rectangle(
                _searchTemplate.DialogOptionsSearchArea.Width.From,
                _searchTemplate.DialogOptionsSearchArea.Height.From,
                _searchTemplate.DialogOptionsSearchArea.Width.To - _searchTemplate.DialogOptionsSearchArea.Width.From,
                _searchTemplate.DialogOptionsSearchArea.Height.To - _searchTemplate.DialogOptionsSearchArea.Height.From));

        using var image = _bitmapUtils.ToGrayScale(croppedArea);

        image.Save("Cut.png");

        for (var y = 0; y < image.Height; y++)
        {
            // Top outline
            var (isTopFound, topOutline) = GetTopOutline(image, y, threshold);
            if (isTopFound == false)
            {
                y = topOutline + 1;
                continue;
            }

            if (topOutline + _searchTemplate.OutlineAreaHeight > image.Height) break;
            
            // Bottom outline
            var (isBottomFound, bottomOutline) = GetBottomOutline(image, topOutline, threshold);
            if (isBottomFound == false)
            {
                y = bottomOutline + 1;
                continue;
            }

            // Center is empty
            if (GetYOutline(image, 
                    _searchTemplate.HorizontalOutlineSearchRangeX.From,
                    _searchTemplate.HorizontalOutlineSearchRangeX.To,
                    topOutline + _searchTemplate.CenterOutlineSearchRangeY.From,
                    topOutline + _searchTemplate.CenterOutlineSearchRangeY.To,
                    0.5) != -1)
            {
                y = bottomOutline;
                continue;
            }

            // Left outline
            var top = topOutline + ((bottomOutline - topOutline) / 2) - (_searchTemplate.VerticalOutlineSearchHeight / 2);
            var verticalOutline = GetVerticalOutline(image,
                _searchTemplate.VerticalOutlineSearchRangeX.From,
                top,
                _searchTemplate.VerticalOutlineSearchRangeX.To,
                top + _searchTemplate.VerticalOutlineSearchHeight,
                threshold);

            if (verticalOutline == -1)
            {
                y = bottomOutline;
                continue;
            }

            // Wrap everything into area
            // Test area
            var area = new Rectangle(
                verticalOutline,
                topOutline,
                image.Width,
                bottomOutline - topOutline);
            dialogOptionsList.Add(area);

            /* Client Area
            var area = new Rectangle(
                verticalOutline - _searchTemplate.BackgroundPadding,
                topOutline - _searchTemplate.BackgroundPadding,
                _searchTemplate.Width,
                bottomOutline - topOutline + _searchTemplate.BackgroundPadding * 2);
            dialogOptionsList.Add(area);
            */

            y = area.Bottom + _searchTemplate.Gap;
        }

        DrawRectangles(image, dialogOptionsList);
        image.Save("Result.png");

        return dialogOptionsList;
    }

    private (bool, int) GetTopOutline(Bitmap image, int y, double threshold)
    {
        var maxSearchPoint = y + _searchTemplate.TopOutlineSearchRangeY.To;
        if (maxSearchPoint >= image.Height) return (false, maxSearchPoint);

        var firstOutlinePosition = GetYOutline(image,
            _searchTemplate.HorizontalOutlineSearchRangeX.From,
            _searchTemplate.HorizontalOutlineSearchRangeX.To,
            y + _searchTemplate.TopOutlineSearchRangeY.From,
            maxSearchPoint, threshold);

        if (firstOutlinePosition == -1) return (false, y + _searchTemplate.TopOutlineSearchRangeY.To);

        var ordinalLinePosition = GetYOrdinal(image,
            _searchTemplate.HorizontalOutlineSearchRangeX.From,
            _searchTemplate.HorizontalOutlineSearchRangeX.To,
            firstOutlinePosition + 1,
            maxSearchPoint, threshold);

        return (true, ordinalLinePosition == -1 ? maxSearchPoint : ordinalLinePosition - 1);
    }

    private (bool, int) GetBottomOutline(Bitmap image, int y, double threshold)
    {
        var maxSearchPoint = y + _searchTemplate.BottomOutlineSearchRangeY.To;
        if (maxSearchPoint >= image.Height) return (false, maxSearchPoint);

        var firstOutlinePosition = GetYOutline(image,
            _searchTemplate.HorizontalOutlineSearchRangeX.From,
            _searchTemplate.HorizontalOutlineSearchRangeX.To,
            y + _searchTemplate.BottomOutlineSearchRangeY.From,
            maxSearchPoint, threshold);

        if (firstOutlinePosition == -1) return (false, y);

        var ordinalLinePosition = GetYOrdinal(image,
            _searchTemplate.HorizontalOutlineSearchRangeX.From,
            _searchTemplate.HorizontalOutlineSearchRangeX.To,
            firstOutlinePosition + 1,
            maxSearchPoint, threshold);

        return (true, ordinalLinePosition == -1 ? maxSearchPoint : ordinalLinePosition - 1);
    }

    private void DrawRectangles(Image bitmap, List<Rectangle> rectangles)
    {
        if (!rectangles.Any()) return;

        using var graphics = Graphics.FromImage(bitmap);
        using var pen = new Pen(Color.LimeGreen, 1);
        rectangles.ForEach(r => graphics.DrawRectangle(pen, r));
    }

    private int GetVerticalOutline(Bitmap image, int x, int y, int maxX, int maxY, double threshold)
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
            if (IsGrayOutline(image.GetPixel(x, y))) resultPoints++;
        }

        return resultPoints >= totalPoints * threshold;
    }

    private int GetYOutline(Bitmap image, int x, int maxX, int y, int maxY, double threshold)
    {
        for (var i = y; i <= maxY; i++)
        {
            if (IsHorizontalLineOutline(image, x, i, maxX, threshold)) return i;
        }
        return -1;
    }

    private int GetYOrdinal(Bitmap image, int x, int maxX, int y, int maxY, double threshold)
    {
        for (var i = y; i <= maxY; i++)
        {
            if (IsHorizontalLineOutline(image, x, i, maxX, threshold) == false) return i;
        }
        return -1;
    }

    private bool IsHorizontalLineOutline(Bitmap image, int x, int y, int maxX, double threshold)
    {
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            if (IsGrayOutline(image.GetPixel(i, y))) resultPoints++;
        }

        return resultPoints >= (maxX - x) * threshold;
    }

    private bool IsGrayOutline(Color color)
    {
        return _searchTemplate.GrayColorRange.From <= color.R && color.R <= _searchTemplate.GrayColorRange.To;
    }
}
