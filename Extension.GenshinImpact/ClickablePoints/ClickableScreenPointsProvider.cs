using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace Extension.GenshinImpact.ClickablePoints;

public class ClickableScreenPointsProvider
{
    public List<ClickablePrecisePointDto> Get(Size clientSize)
    {
        var concretePoints = new ClickablePointsMapper().Map(clientSize);
        return new List<ClickablePrecisePointDto>()
        {
            new("autoplay", concretePoints.AutoPlay)
        };
    }
}
