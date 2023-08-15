using System.Drawing;

namespace Extension.GenshinImpact.ClickablePoints;

public class ClickablePointsMapper
{
    public PointTemplate Map(Size clientSize)
    {
        var dynamicPointTemplate = new DynamicPointTemplate();
        return new PointTemplate
        {
            AutoPlay = new Point(
                (int)(dynamicPointTemplate.AutoPlay.X * clientSize.Width),
                (int)(dynamicPointTemplate.AutoPlay.Y * clientSize.Height))
        };
    }
}
