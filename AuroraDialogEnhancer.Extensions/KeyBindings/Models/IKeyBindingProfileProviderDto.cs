using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

namespace AuroraDialogEnhancerExtensions.KeyBindings.Models;

public interface IKeyBindingProfileProviderDto
{
    public KeyBindingProfileDto GetKeyBindingProfileDto();

    public List<InteractionPointVmDto> GetInteractionPointsVmDto();
}
