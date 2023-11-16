using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;

public class InteractionPrecisePointMapper : IMapper<InteractionPrecisePointDto, InteractionPrecisePoint>
{
    public InteractionPrecisePoint Map(InteractionPrecisePointDto obj)
    {
        return new InteractionPrecisePoint(obj.Id, obj.Point);
    }
}
