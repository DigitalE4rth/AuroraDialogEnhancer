using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class PresetData
{
    public Rectangle            SpeakerNameArea    { get; set; }
    public Rectangle            DialogOptionsArea  { get; set; }
    public CursorPositionConfig CursorPositionData { get; set; } = new();

    public PresetData(Rectangle            speakerNameArea,
                      Rectangle            dialogOptionsArea,
                      CursorPositionConfig cursorPositionData)
    {
        SpeakerNameArea    = speakerNameArea;
        DialogOptionsArea  = dialogOptionsArea;
        CursorPositionData = cursorPositionData;
    }

    public PresetData()
    {
    }
}
