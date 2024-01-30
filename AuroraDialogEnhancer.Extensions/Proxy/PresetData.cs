namespace AuroraDialogEnhancerExtensions.Proxy;

public class PresetData
{
    public DialogDetectionConfig DialogDetectionConfig { get; set; } = new();
    public CursorConfigBase      CursorData            { get; set; } = new CursorConfigDefault();

    public PresetData(DialogDetectionConfig dialogDetectionConfig,
                      CursorConfigBase      cursorData)
    {
        DialogDetectionConfig = dialogDetectionConfig;
        CursorData            = cursorData;
    }

    public PresetData()
    {
    }
}
