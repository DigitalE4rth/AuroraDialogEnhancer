using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

namespace AuroraDialogEnhancerExtensions.KeyBindings.Models;

internal class KeyBindingProfileProviderEmpty : IKeyBindingProfileProviderDto
{
    public KeyBindingProfileDto GetKeyBindingProfileDto() => new KeyBindingProfileDtoEmpty();

    public List<InteractionPointVmDto> GetInteractionPointsVmDto() => new(0);
}
