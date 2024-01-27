using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

namespace Extension.HonkaiStarRail.InteractionPoints;

public class InteractionScreenPointsProvider
{
    public List<InteractionPrecisePointDto> Get(Size clientSize)
    {
        var concretePoints = new InteractionPointsMapper().Map(clientSize);
        return new List<InteractionPrecisePointDto>
        {
            new("autoplay", concretePoints.AutoPlay),
            new("hideui", concretePoints.HideUi),
            new("fullscreenpopup", concretePoints.FullScreenPopUp)
        };
    }
}
