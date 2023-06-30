using System.Collections.Generic;
using System.Drawing;
using Point = System.Drawing.Point;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

internal class GameCvDialogOptionFinder
{
    private readonly BitmapUtils _bitmapUtils;
    private readonly GameCvData  _gameCvData;
    private DialogOptionSearchArea _dialogOptionSearchArea;
    private Rectangle _searchArea;

    public GameCvDialogOptionFinder()
    {
        _bitmapUtils = new BitmapUtils();
        _gameCvData = new GameCvData();
        _dialogOptionSearchArea = null!;
    }

    public void Initialize(Size clientSize)
    {
        var doHeight = (int) (clientSize.Width * _gameCvData.DialogOptionHeightInPercentage);
        var searchAreaStart = (int) (clientSize.Width * _gameCvData.DialogOptionsSearchWidthStartInPercentage);
        var searchAreaEnd   = (int) (searchAreaStart + (doHeight + doHeight * 0.5));
        var searchAreaWidth = searchAreaEnd - searchAreaStart;

        _searchArea = new Rectangle(searchAreaStart, 0, searchAreaWidth, clientSize.Height);
        _dialogOptionSearchArea = new DialogOptionSearchArea(searchAreaWidth, doHeight);
    }

    public List<Point> GetDialogOptions(Bitmap rawImage)
    {
        var result = new List<Point>();
        using var searchArea = _bitmapUtils.GetArea(rawImage, _searchArea);
        using var processingImage = _bitmapUtils.ToGrayScale(searchArea);
        processingImage.Save("GrayTest.png");

        for (var y = 0; y < processingImage.Height; y++)
        {
            // Top line is outline (by X)
            if (IsLineOutline(processingImage, _dialogOptionSearchArea.WidthFifty, y, processingImage.Width) == false) continue;
            if (y + _dialogOptionSearchArea.OutlineArea.Height > processingImage.Height) break;

            // Bottom line is outline (by X)
            var bottomLineY = GetYOutlineLinePosition(processingImage, 
                _dialogOptionSearchArea.WidthFifty, y + _dialogOptionSearchArea.OutlineArea.HeightNinetyTwo,
                processingImage.Width, _dialogOptionSearchArea.OutlineArea.BottomLineSearchHeight);

            if (bottomLineY == -1) continue;

            // Center line in NOT outline (by X)
            if (IsLineOutline(processingImage,
                    _dialogOptionSearchArea.WidthFifty, y + _dialogOptionSearchArea.OutlineArea.HeightFifty,
                    processingImage.Width,
                    0.5))
            {
                y = bottomLineY;
                continue;
            }

            // Left line has outline points (by Y)
            var leftLineX = GetXOutlineLinePosition(processingImage,
                0, y + _dialogOptionSearchArea.OutlineArea.HeightFifty,
                _dialogOptionSearchArea.WidthOne);

            if (leftLineX == -1) continue;

            result.Add(new Point(_searchArea.X + leftLineX, y));

            y = bottomLineY + 1;
        }

        return result;
    }

    private int GetXOutlineLinePosition(Bitmap image, int x, int y, int maxX)
    {
        for (var i = x; i < maxX; i++)
        {
            if (IsGrayOutline(image.GetPixel(i, y)) &&
                IsGrayOutline(image.GetPixel(i, y - 1)) &&
                IsGrayOutline(image.GetPixel(i, y + 1))) return i;
        }

        return -1;
    }
    
    private int GetYOutlineLinePosition(Bitmap image, int x, int y, int maxX, int height)
    {
        for (var i = y; i < y + height; i++)
        {
            if (IsLineOutline(image, x, i, maxX)) return i;
        }
        return -1;
    }

    private bool IsLineOutline(Bitmap image, int x, int y, int maxX, double threshold = 1)
    {
        var totalPoints = maxX - x;
        var resultPoints = 0;

        for (var i = x; i < maxX; i++)
        {
            if (!IsGrayOutline(image.GetPixel(i, y))) continue;
            resultPoints++;
        }

        return resultPoints >= totalPoints * threshold;
    }

    private bool IsGrayOutline(Color color)
    {
        return _gameCvData.LowGrayColor <= color.R && color.R <= _gameCvData.HighGrayColor;
    }
}
