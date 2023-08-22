using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionFinderData
{
    public Rectangle SpeakerNameArea { get; } = Rectangle.Empty;

    public Rectangle DialogOptionsArea { get; } = Rectangle.Empty;

    public Point InitialCursorPosition { get; } = Point.Empty;

    public DialogOptionFinderData(Rectangle speakerNameArea, Rectangle dialogOptionsArea, Point initialCursorPosition)
    {
        SpeakerNameArea = speakerNameArea;
        DialogOptionsArea = dialogOptionsArea;
        InitialCursorPosition = initialCursorPosition;
    }

    public DialogOptionFinderData()
    {
    }
}
