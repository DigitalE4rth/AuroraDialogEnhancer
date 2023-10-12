using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ClickablePoints;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.KeyBindings.ClickablePoints;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Mappers;

public class ClickablePrecisePointMapper : IMapper<ClickablePrecisePointDto, ClickablePrecisePoint>
{
    public ClickablePrecisePoint Map(ClickablePrecisePointDto obj)
    {
        return new ClickablePrecisePoint(obj.Id, obj.Point);
    }
}
