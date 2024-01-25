using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionFinderData
{
    public Rectangle          SpeakerNameArea;
    public Rectangle          DialogOptionsArea;
    public CursorPositionData CursorPositionData = new();
    public double             CursorSmoothingPercentage;

    public DialogOptionFinderData(Rectangle          speakerNameArea, 
                                  Rectangle          dialogOptionsArea, 
                                  CursorPositionData cursorPositionData, 
                                  double             cursorSmoothingPercentage)
    {
        SpeakerNameArea           = speakerNameArea;
        DialogOptionsArea         = dialogOptionsArea;
        CursorPositionData        = cursorPositionData;
        CursorSmoothingPercentage = cursorSmoothingPercentage;
    }

    public DialogOptionFinderData()
    {
    }
}
