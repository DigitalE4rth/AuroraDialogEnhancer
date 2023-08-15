using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace Extension.GenshinImpact.ClickablePoints;

public class ClickableScreenPointsProvider
{
    public List<ClickableScreenPointDto> Get(Size clientSize)
    {
        var concretePoints = new ClickablePointsMapper().Map(clientSize);
        return new List<ClickableScreenPointDto>()
        {
            new("autoplay", concretePoints.AutoPlay)
        };
    }
}
