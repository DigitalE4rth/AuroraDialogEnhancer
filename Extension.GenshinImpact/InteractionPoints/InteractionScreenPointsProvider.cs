using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

namespace Extension.GenshinImpact.InteractionPoints;

public class InteractionScreenPointsProvider
{
    public List<InteractionPrecisePointDto> Get(Size clientSize)
    {
        var concretePoints = new InteractionPointsMapper().Map(clientSize);
        return new List<InteractionPrecisePointDto>
        {
            new("autoplay", concretePoints.AutoPlay),
            new("fullscreenpopup", concretePoints.FullScreenPopUp)
        };
    }
}
