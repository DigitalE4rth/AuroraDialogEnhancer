namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

internal class OutlineArea
{
    public int Height { get; }
    public int HeightFifty { get; }
    public int HeightNinetyTwo { get; }
    public int BottomLineSearchHeight { get; }

    public OutlineArea(int height)
    {
        Height = height;
        HeightFifty = height / 2;
        HeightNinetyTwo = (int) (height * 0.92);
        BottomLineSearchHeight = Height - HeightNinetyTwo;
    }
}
