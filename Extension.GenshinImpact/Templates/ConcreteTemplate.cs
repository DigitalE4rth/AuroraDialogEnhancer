using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

internal class ConcreteTemplate
{
    public Area  DialogOptionsSearchAreaPercentage = new(0, 0, 0, 1);
    public Range VerticalOutlineSearchWidth = new(0, 0);
    public Range HorizontalOutlineSearchWidth = new(0, 1);
    public Range GrayColorRange = new(76, 109);

    public int Height = 0;
    public int Width = 0;
    public int Gap = 0;
    public int BackGroundPadding = 0;
    public int OutlineAreaHeight = 0;
}
