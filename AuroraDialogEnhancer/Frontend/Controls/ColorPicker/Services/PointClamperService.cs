using System;
using System.Windows;
using System.Windows.Input;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Services;

internal class PointClamperService
{
    /// <summary>
    /// Clamps the point to the inside of the element
    /// </summary>
    /// <param name="element">The element</param>
    /// <returns>A point inside the element</returns>
    internal Point Clamp(FrameworkElement element)
    {
        var position = Mouse.GetPosition(element);
        position.X = Math.Min(Math.Max(0, position.X), element.ActualWidth);
        position.Y = Math.Min(Math.Max(0, position.Y), element.ActualHeight);
        return position;
    }
}
