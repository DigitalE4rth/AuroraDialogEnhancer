using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Utils;

public class ColorWrapper<T> where T : IColor
{
    public ColorRange<T>   IconColor  { get; set; }
    public ColorRange<T>[] TextColors { get; set; }

    public ColorWrapper(ColorRange<T> icon, ColorRange<T>[] textColors)
    {
        IconColor  = icon;
        TextColors = textColors;
    }
}
