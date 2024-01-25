using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionFinderData
{
    public Rectangle          SpeakerNameArea;
    public Rectangle          DialogOptionsArea;
    public CursorPositionData CursorPositionData = new();

    public DialogOptionFinderData(Rectangle          speakerNameArea, 
                                  Rectangle          dialogOptionsArea, 
                                  CursorPositionData cursorPositionData)
    {
        SpeakerNameArea    = speakerNameArea;
        DialogOptionsArea  = dialogOptionsArea;
        CursorPositionData = cursorPositionData;
    }

    public DialogOptionFinderData()
    {
    }
}
