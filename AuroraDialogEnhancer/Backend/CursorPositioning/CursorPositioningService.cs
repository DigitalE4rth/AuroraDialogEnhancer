using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancerExtensions.Proxy;

namespace AuroraDialogEnhancer.Backend.CursorPositioning;

public class CursorPositioningService
{
    private readonly ProcessDataProvider _processDataProvider;

    private Point  _cursorPosition;
    private double _dynamicCursorPositionY;
    private double _placementSmoothness;
    private int    _hiddenCursorPositionY;
    private double _movementSmoothness;

    public CursorPositioningService(ProcessDataProvider processDataProvider)
    {
        _processDataProvider = processDataProvider;
    }

    public void InitialCursorData(CursorConfigBase configBase)
    {
        _cursorPosition         = new Point(configBase.InitialPositionX, 0);
        _dynamicCursorPositionY = configBase.InitialPosition.Y;
        _hiddenCursorPositionY  = configBase.HiddenCursorPositionY;
        _placementSmoothness    = configBase.PlacementSmoothness;
        _movementSmoothness     = configBase.MovementSmoothness;
    }

    public void ApplyRelative(Rectangle dialogOption)
    {
        _cursorPosition = new Point(
            Cursor.Position.X - _processDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X - dialogOption.X,
            Cursor.Position.Y - _processDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y   - dialogOption.Y);

        var newDynamicCursorPositionY = _cursorPosition.Y / ((double)dialogOption.Height - 1);
        if (Math.Abs(_dynamicCursorPositionY - newDynamicCursorPositionY) <= _placementSmoothness) return;
        _dynamicCursorPositionY = newDynamicCursorPositionY;
    }

    public void ApplyRelativeX(Rectangle dialogOption)
    {
        _cursorPosition = _cursorPosition with { X = Cursor.Position.X - _processDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X - dialogOption.X };
    }

    public void ApplyRelativeCursorPosition(CursorPositionInfo cursorPositionInfo, List<Rectangle> dialogOptions)
    {
        if (!cursorPositionInfo.IsWithinBoundaries) return;

        if (cursorPositionInfo.HighlightedIndex != -1)
        {
            ApplyRelative(dialogOptions[cursorPositionInfo.HighlightedIndex]);
            return;
        }

        ApplyRelativeX(dialogOptions[0]);
    }

    public Point GetTargetCursorPlacement(Rectangle dialogOption)
    {
        return new Point(_processDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + dialogOption.X + _cursorPosition.X,
                         _processDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y   + dialogOption.Y + (int) Math.Floor(dialogOption.Height * _dynamicCursorPositionY));
    }

    public Point GetDefaultTargetCursorPlacement(Rectangle dialogOption)
    {
        return new Point(_processDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + dialogOption.X + _cursorPosition.X,
                         _processDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y   + dialogOption.Y + (int) Math.Floor(dialogOption.Height * _dynamicCursorPositionY));
    }

    public bool IsCursorInsideClient()
    {
        return Cursor.Position.X >= _processDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X &&
               Cursor.Position.X <= _processDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.X + _processDataProvider.Data.GameWindowInfo.ClientRectangle.Width &&
               Cursor.Position.Y >= _processDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y &&
               Cursor.Position.Y <= _processDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + _processDataProvider.Data.GameWindowInfo.ClientRectangle.Height;
    }

    public CursorPositionInfo GetPositionByDialogOptions(List<Rectangle> dialogOption) => GetPositionByDialogOptions(dialogOption, Cursor.Position);
    public CursorPositionInfo GetPositionByDialogOptions(List<Rectangle> dialogOption, Point cursorPosition)
    {
        var relativeCursorPosition = new Point(
            cursorPosition.X - _processDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X,
            cursorPosition.Y - _processDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y);

        // Not within boundaries
        if (relativeCursorPosition.X < dialogOption[0].Left 
            ||
            relativeCursorPosition.X > dialogOption[0].Right)
        {
            return new CursorPositionInfo(-1, -1, -1);
        }

        // Upper area of first
        if (relativeCursorPosition.X >= dialogOption[0].Left  &&
            relativeCursorPosition.X <= dialogOption[0].Right &&
            relativeCursorPosition.Y >= _processDataProvider.Data.GameWindowInfo.ClientRectangle.Top &&
            relativeCursorPosition.Y < dialogOption[0].Top)
        {
            return new CursorPositionInfo(-1, -1, 0);
        }

        // Lower area of last
        if (relativeCursorPosition.X >= dialogOption[dialogOption.Count - 1].Left  &&
            relativeCursorPosition.X <= dialogOption[dialogOption.Count - 1].Right &&
            relativeCursorPosition.Y <= _processDataProvider.Data.GameWindowInfo.ClientRectangle.Bottom &&
            relativeCursorPosition.Y > dialogOption[dialogOption.Count - 1].Bottom)
        {
            return new CursorPositionInfo(dialogOption.Count - 1, -1, -1);
        }

        var closestUpperIndex = -1;
        var closestLowerIndex = -1;
        var highlightedIndex  = GetHighlightedIndex(dialogOption, relativeCursorPosition);

        if (highlightedIndex != -1)
        {
            if (highlightedIndex > 0 && highlightedIndex < dialogOption.Count - 1)
            {
                closestUpperIndex = highlightedIndex - 1;
                closestLowerIndex = highlightedIndex + 1;
                return new CursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
            }

            if (highlightedIndex == 0)
            {
                closestUpperIndex = -1;
                closestLowerIndex = dialogOption.Count > 1 ? 1 : -1;
            }

            if (highlightedIndex == dialogOption.Count - 1)
            {
                closestLowerIndex = -1;
            }

            return new CursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
        }

        closestLowerIndex = GetHighlightedIndex(dialogOption, new Point(relativeCursorPosition.X, relativeCursorPosition.Y));
        closestUpperIndex = GetHighlightedIndex(dialogOption, new Point(relativeCursorPosition.X, relativeCursorPosition.Y));

        return new CursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
    }

    private int GetHighlightedIndex(IReadOnlyList<Rectangle> dialogOption, Point relativeCursorPosition)
    {
        var highlightedIndex = -1;
        for (var i = 0; i < dialogOption.Count; i++)
        {
            // Within current boundaries
            if (relativeCursorPosition.X < dialogOption[i].Left  ||
                relativeCursorPosition.X > dialogOption[i].Right ||
                relativeCursorPosition.Y < dialogOption[i].Top   ||
                relativeCursorPosition.Y > dialogOption[i].Bottom) 
                continue;

            highlightedIndex = i;
            break;
        }

        return highlightedIndex;
    }

    public Point GetAbsoluteFromRelativePoint(Point relativePoint)
    {
        return new Point(_processDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + relativePoint.X,
                         _processDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y   + relativePoint.Y);
    }

    public void Hide() => HideByY();
    private void HideByY()
    {
        Cursor.Position = (Cursor.Position with
        {
            Y = _processDataProvider.Data!.GameWindowInfo!.BottomYPoint - _hiddenCursorPositionY
        });
    }

    public void SetCursorPositionWithAnimation(Point newPosition)
    {
        while (true)
        {
            var position = Cursor.Position;

            var diffX = newPosition.X - position.X;
            var diffY = newPosition.Y - position.Y;

            for (var i = 0; i <= _movementSmoothness; i++)
            {
                var x = position.X + (diffX / _movementSmoothness * i);
                var y = position.Y + (diffY / _movementSmoothness * i);
                Cursor.Position = new Point((int)x, (int)y);
                Thread.Sleep(1);
            }

            if (Cursor.Position != newPosition) continue;

            break;
        }
    }

    public Point GetRelatedNormalizedPoint(Rectangle targetDialogOption)
    {
        return Cursor.Position with { 
            Y = _processDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.Y
                + targetDialogOption.Y 
                + (int) Math.Floor(targetDialogOption.Height * _dynamicCursorPositionY) };
    }
}
