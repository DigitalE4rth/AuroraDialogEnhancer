namespace AuroraDialogEnhancerExtensions.Dimensions;

public class Area : AreaBase<Range>
{
    public Area(Range width, Range height) : base(width, height)
    {
    }

    public Area(int widthFrom, int widthTo, int heightFrom, int heightTo) 
        : base(new Range(widthFrom, widthTo), new Range(heightFrom, heightTo))
    {
    }

    public Area() : base(new Range(), new Range())
    {
    }
}
