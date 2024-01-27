using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Utils;

public class ColorWrapper
{
    public ColorRange IconColor { get; set; }

    public ColorRange[] TextColors { get; set; }

    public ColorWrapper(ColorRange icon, ColorRange[] textColors)
    {
        IconColor  = icon;
        TextColors = textColors;
    }
}
