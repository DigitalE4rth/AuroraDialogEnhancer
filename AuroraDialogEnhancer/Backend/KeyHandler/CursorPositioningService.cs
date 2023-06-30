using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancerExtensions.KeyBinding;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class CursorPositioningService
{
    private readonly HookedGameDataProvider _hookedGameDataProvider;
    private Action HideAction { get; set; }

    public CursorPositioningService(HookedGameDataProvider hookedGameDataProvider)
    {
        _hookedGameDataProvider = hookedGameDataProvider;
        HideAction = HideByY;
    }

    public void SetHiddenSetting(EHiddenCursorSetting cursorSetting)
    {
        HideAction = cursorSetting == EHiddenCursorSetting.YCoordinate ? HideByY : HideByCoordinates;
    }

    public void ApplyRelative(Point dialogOptionPoint)
    {
        _hookedGameDataProvider.Data!.CvPreset!.DialogOptionInitialCursorPosition = new Point(
            Cursor.Position.X - _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X - dialogOptionPoint.X,
            Cursor.Position.Y - _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y - dialogOptionPoint.Y);
    }

    public void ApplyRelativeX(Point dialogOptionPoint)
    {
        _hookedGameDataProvider.Data!.CvPreset!.DialogOptionInitialCursorPosition = new Point(
            Cursor.Position.X - _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X - dialogOptionPoint.X,
            _hookedGameDataProvider.Data!.CvPreset!.DialogOptionInitialCursorPosition.Y);
    }


    public Point GetTargetCursorPlacement(Point dialogOptionPoint)
    {
        return new Point(_hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + dialogOptionPoint.X + _hookedGameDataProvider.Data!.CvPreset!.DialogOptionInitialCursorPosition.X,
                         _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + dialogOptionPoint.Y + _hookedGameDataProvider.Data!.CvPreset!.DialogOptionInitialCursorPosition.Y);

    }

    public bool IsClickedInsideClient()
    {
        return Cursor.Position.X >= _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X &&
               Cursor.Position.X <= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.X + _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Width &&
               Cursor.Position.Y >= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y &&
               Cursor.Position.Y <= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Height;
    }

    public DialogOptionCursorPositionInfo GetPositionByDialogOptions(List<Point> dialogOptionPoints, Point cursorPosition = default)
    {
        if (cursorPosition == Point.Empty)
        {
            cursorPosition = Cursor.Position;
        }

        var relativeCursorPosition = new Point(
            cursorPosition.X - _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X,
            cursorPosition.Y - _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y);

        // Not within boundaries
        if (relativeCursorPosition.X < dialogOptionPoints[0].X + _hookedGameDataProvider.Data.CvPreset!.DialogOptionRegion.X ||
            relativeCursorPosition.X > dialogOptionPoints[0].X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Width)
        {
            return new DialogOptionCursorPositionInfo(-1, -1, -1);
        }

        // Upper area of first
        if (relativeCursorPosition.X >= dialogOptionPoints[0].X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.X &&
            relativeCursorPosition.X <= dialogOptionPoints[0].X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Width &&
            relativeCursorPosition.Y >= 0 &&
            relativeCursorPosition.Y < dialogOptionPoints[0].Y + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Y)
        {
            return new DialogOptionCursorPositionInfo(-1, -1, 0);
        }

        // Lower area of last
        if (relativeCursorPosition.X >= dialogOptionPoints[dialogOptionPoints.Count - 1].X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.X &&
            relativeCursorPosition.X <= dialogOptionPoints[dialogOptionPoints.Count - 1].X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Width &&
            relativeCursorPosition.Y <= _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Height &&
            relativeCursorPosition.Y > dialogOptionPoints[dialogOptionPoints.Count - 1].Y + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Y + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Height)
        {
            return new DialogOptionCursorPositionInfo(dialogOptionPoints.Count - 1, -1, -1);
        }

        var closestUpperIndex = -1;
        var closestLowerIndex = -1;
        var highlightedIndex = GetHighlightedIndex(dialogOptionPoints, relativeCursorPosition);

        if (highlightedIndex != -1)
        {
            if (highlightedIndex > 0 && highlightedIndex < dialogOptionPoints.Count - 1)
            {
                closestUpperIndex = highlightedIndex - 1;
                closestLowerIndex = highlightedIndex + 1;
                return new DialogOptionCursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
            }

            if (highlightedIndex == 0)
            {
                closestUpperIndex = -1;
            }

            if (highlightedIndex == dialogOptionPoints.Count - 1)
            {
                closestLowerIndex = -1;
            }

            return new DialogOptionCursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
        }

        closestLowerIndex = GetHighlightedIndex(dialogOptionPoints, new Point(relativeCursorPosition.X, relativeCursorPosition.Y + _hookedGameDataProvider.Data.CvPreset.DialogOptionGap));
        closestUpperIndex = GetHighlightedIndex(dialogOptionPoints, new Point(relativeCursorPosition.X, relativeCursorPosition.Y - _hookedGameDataProvider.Data.CvPreset.DialogOptionGap));

        return new DialogOptionCursorPositionInfo(closestUpperIndex, highlightedIndex, closestLowerIndex);
    }

    private int GetHighlightedIndex(IReadOnlyList<Point> dialogOptionPoints, Point relativeCursorPosition)
    {
        var highlightedIndex = -1;
        for (var i = 0; i < dialogOptionPoints.Count; i++)
        {
            // Within current boundaries
            if (relativeCursorPosition.X >= dialogOptionPoints[i].X + _hookedGameDataProvider.Data!.CvPreset!.DialogOptionRegion.X &&
                relativeCursorPosition.X <= dialogOptionPoints[i].X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.X + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Width &&
                relativeCursorPosition.Y >= dialogOptionPoints[i].Y + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Y &&
                relativeCursorPosition.Y <= dialogOptionPoints[i].Y + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Y + _hookedGameDataProvider.Data.CvPreset.DialogOptionRegion.Height)
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

    public void Hide() => HideAction();

    private void HideByCoordinates()
    {
        Cursor.Position = new Point(
            _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + _hookedGameDataProvider.Data.CvPreset!.HiddenCursorLocation.X,
            _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + _hookedGameDataProvider.Data.CvPreset.HiddenCursorLocation.Y);
    }

    private void HideByY()
    {
        Cursor.Position = Cursor.Position with
        {
            Y = _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.Y 
                + _hookedGameDataProvider.Data.CvPreset!.HiddenCursorLocation.Y
        };
    }

    public Point GetRelatedNormalizedPoint(Point currentSourcePoint, Point desiredSourcePoint)
    {
        return new Point(Cursor.Position.X, (Cursor.Position.Y - currentSourcePoint.Y) + desiredSourcePoint.Y);
    }
}
