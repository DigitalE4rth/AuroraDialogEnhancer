using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;
using AuroraDialogEnhancerExtensions.KeyBindings.Models;
using Extension.HonkaiStarRail.InteractionPoints;

namespace Extension.HonkaiStarRail.KeyBindings;

public class KeyBindingProfileProvider : IKeyBindingProfileProviderDto
{
    public KeyBindingProfileDto GetKeyBindingProfileDto() => new KeyBindingProfile();

    public List<InteractionPointVmDto> GetInteractionPointsVmDto() => new InteractionPointViewModelProvider().Get();
}
