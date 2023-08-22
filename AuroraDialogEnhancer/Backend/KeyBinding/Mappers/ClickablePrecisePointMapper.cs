using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Mappers;

public class ClickablePrecisePointMapper : IMapper<ClickablePrecisePointDto, ClickablePrecisePoint>
{
    public ClickablePrecisePoint Map(ClickablePrecisePointDto obj)
    {
        return new ClickablePrecisePoint(obj.Id, obj.Point);
    }
}
