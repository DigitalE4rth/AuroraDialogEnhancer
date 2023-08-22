using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace Extension.GenshinImpact.ClickablePoints;

public class ClickableScreenPointsProvider
{
    public List<ClickablePrecisePoint> Get(Size clientSize)
    {
        var concretePoints = new ClickablePointsMapper().Map(clientSize);
        return new List<ClickablePrecisePoint>()
        {
            new("autoplay", concretePoints.AutoPlay)
        };
    }
}
