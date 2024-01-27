using System;
using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogDetectionConfig
{
    public Rectangle[] IndicationAreas   { get; set; } = Array.Empty<Rectangle>();
    public Rectangle   DialogOptionsArea { get; set; }

    public DialogDetectionConfig(Rectangle[] indicationAreas,
                                 Rectangle   dialogOptionsArea)
    {
        IndicationAreas   = indicationAreas;
        DialogOptionsArea = dialogOptionsArea;
    }

    public DialogDetectionConfig()
    {
    }
}
