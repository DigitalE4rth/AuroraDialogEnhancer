namespace AuroraDialogEnhancer.Frontend.Forms.Debug.CvData;

internal class ConcreteTemplate
{
    public SearchArea<int>  DialogOptionsSearchAreaPercentage = new(0, 0, 0, 1);
    public SearchRange<int> VerticalOutlineSearchWidth = new(0, 0);
    public SearchRange<int> HorizontalOutlineSearchWidth = new(0, 1);
    public SearchRange<int> GrayColorRange = new(76, 109);

    public int Height = 0;
    public int Width = 0;
    public int Gap = 0;
    public int BackGroundPadding = 0;
    public int OutlineAreaHeight = 0;
}
