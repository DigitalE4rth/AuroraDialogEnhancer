namespace AuroraDialogEnhancerExtensions.Dimensions;

public class AreaDynamic : AreaBase<RangeDynamic>
{
    public AreaDynamic(RangeDynamic width, RangeDynamic height) : base(width, height)
    {
    }

    public AreaDynamic(double widthFrom, double widthTo, double heightFrom, double heightTo) 
        : base(new RangeDynamic(widthFrom, widthTo), new RangeDynamic(heightFrom, heightTo))
    {
    }
}
