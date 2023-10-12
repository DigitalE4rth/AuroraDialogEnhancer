using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings.ClickablePoints;

namespace AuroraDialogEnhancerExtensions.KeyBindings.Models;

internal class KeyBindingProfileProviderEmpty : IKeyBindingProfileProviderDto
{
    public KeyBindingProfileDto GetKeyBindingProfileDto() => new KeyBindingProfileDtoEmpty();

    public List<ClickablePointVmDto> GetClickablePointsVmDto() => new(0);
}
