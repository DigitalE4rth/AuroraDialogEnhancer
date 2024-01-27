namespace AuroraDialogEnhancerExtensions.Proxy;

public class PresetData
{
    public DialogDetectionConfig DialogDetectionConfig { get; set; } = new();
    public CursorPositionConfig  CursorPositionData    { get; set; } = new();

    public PresetData(DialogDetectionConfig dialogDetectionConfig,
                      CursorPositionConfig  cursorPositionData)
    {
        DialogDetectionConfig = dialogDetectionConfig;
        CursorPositionData    = cursorPositionData;
    }

    public PresetData()
    {
    }
}
