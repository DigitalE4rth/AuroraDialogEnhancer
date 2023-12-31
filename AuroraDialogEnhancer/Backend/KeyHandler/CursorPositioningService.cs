﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AuroraDialogEnhancer.Backend.Hooks.Game;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class CursorPositioningService
{
    private readonly HookedGameDataProvider _hookedGameDataProvider;

    private Point _initialCursorPosition = Point.Empty;
    private Point _cursorPosition        = Point.Empty;

    public void SetInitialCursorPosition(Point cursorPosition)
    {
        _cursorPosition        = cursorPosition;
        _initialCursorPosition = cursorPosition;
    }

    public CursorPositioningService(HookedGameDataProvider hookedGameDataProvider)
    {
        _hookedGameDataProvider = hookedGameDataProvider;
    }

    public void ApplyRelative(Rectangle dialogOption)
    {
        _cursorPosition = new Point(
            Cursor.Position.X - _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X - dialogOption.X,
            Cursor.Position.Y - _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y - dialogOption.Y);
    }

    public void ApplyRelativeX(Rectangle dialogOption)
    {
        _cursorPosition = new Point(
            Cursor.Position.X - _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X - dialogOption.X,
            _cursorPosition.Y);
    }


    public Point GetTargetCursorPlacement(Rectangle dialogOption)
    {
        return new Point(_hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + dialogOption.X + _cursorPosition.X,
                         _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + dialogOption.Y + _cursorPosition.Y);
    }

    public Point GetDefaultTargetCursorPlacement(Rectangle dialogOption)
    {
        return new Point(_hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + dialogOption.X + _initialCursorPosition.X,
                         _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + dialogOption.Y + _initialCursorPosition.Y);
    }

    public bool IsCursorInsideClient()
    {
        return Cursor.Position.X >= _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X &&
               Cursor.Position.X <= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.X + _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Width &&
               Cursor.Position.Y >= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y &&
               Cursor.Position.Y <= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Height;
    }

    public DialogOptionCursorPositionInfo GetPositionByDialogOptions(List<Rectangle> dialogOption) => GetPositionByDialogOptions(dialogOption, Cursor.Position);
    public DialogOptionCursorPositionInfo GetPositionByDialogOptions(List<Rectangle> dialogOption, Point cursorPosition)
    {
        var relativeCursorPosition = new Point(
            cursorPosition.X - _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X,
            cursorPosition.Y - _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y);

        // Not within boundaries
        if (relativeCursorPosition.X < dialogOption[0].Left ||
            relativeCursorPosition.X > dialogOption[0].Right)
        {
            return new DialogOptionCursorPositionInfo(-1, -1, -1);
        }

        // Upper area of first
        if (relativeCursorPosition.X >= dialogOption[0].Left &&
            relativeCursorPosition.X <= dialogOption[0].Right &&
            relativeCursorPosition.Y >= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Top &&
            relativeCursorPosition.Y < dialogOption[0].Top)
        {
            return new DialogOptionCursorPositionInfo(-1, -1, 0);
        }

        // Lower area of last
        if (relativeCursorPosition.X >= dialogOption[dialogOption.Count - 1].Left &&
            relativeCursorPosition.X <= dialogOption[dialogOption.Count - 1].Right &&
            relativeCursorPosition.Y <= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Bottom &&
            relativeCursorPosition.Y > dialogOption[dialogOption.Count - 1].Bottom)
        {
            return new DialogOptionCursorPositionInfo(dialogOption.Count - 1, -1, -1);
        }

        var closestUpperIndex = -1;
        var closestLowerIndex = -1;
        var highlightedIndex = GetHighlightedIndex(dialogOption, relativeCursorPosition);

        if (highlightedIndex != -1)
        {
            if (highlightedIndex > 0 && highlightedIndex < dialogOption.Count - 1)
            {
                closestUpperIndex = highlightedIndex - 1;
                closestLowerIndex = highlightedIndex + 1;
                return new DialogOptionCursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
            }

            if (highlightedIndex == 0)
            {
                closestUpperIndex = -1;
            }

            if (highlightedIndex == dialogOption.Count - 1)
            {
                closestLowerIndex = -1;
            }

            return new DialogOptionCursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
        }

        closestLowerIndex = GetHighlightedIndex(dialogOption, new Point(relativeCursorPosition.X, relativeCursorPosition.Y));
        closestUpperIndex = GetHighlightedIndex(dialogOption, new Point(relativeCursorPosition.X, relativeCursorPosition.Y));

        return new DialogOptionCursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
    }

    private int GetHighlightedIndex(IReadOnlyList<Rectangle> dialogOption, Point relativeCursorPosition)
    {
        var highlightedIndex = -1;
        for (var i = 0; i < dialogOption.Count; i++)
        {
            // Within current boundaries
            if (relativeCursorPosition.X >= dialogOption[i].Left &&
                relativeCursorPosition.X <= dialogOption[i].Right &&
                relativeCursorPosition.Y >= dialogOption[i].Top &&
                relativeCursorPosition.Y <= dialogOption[i].Bottom)
            {
                highlightedIndex = i;
                break;
            }
        }

        return highlightedIndex;
    }

    public Point GetAbsoluteFromRelativePoint(Point relativePoint)
    {
        return new Point(_hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + relativePoint.X,
                         _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + relativePoint.Y);
    }

    public void Hide() => HideByY();

    private void HideByY()
    {
        Cursor.Position = Cursor.Position with
        {
            Y = _hookedGameDataProvider.Data!.GameWindowInfo!.BottomYPoint
        };
    }

    public Point GetRelatedNormalizedPoint(Rectangle currentDialogOption, Rectangle desiredDialogOption)
    {
        return new Point(Cursor.Position.X, (Cursor.Position.Y - currentDialogOption.Y) + desiredDialogOption.Y);
    }
}
