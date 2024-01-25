namespace AuroraDialogEnhancerExtensions.Proxy;

public class CursorPositionData
{
    public int    ConcreteX { get; set; }
    public double DynamicY  { get; set; }

    public CursorPositionData(int concreteX, double dynamicY)
    {
        ConcreteX = concreteX;
        DynamicY  = dynamicY;
    }

    public CursorPositionData()
    {
    }
}
