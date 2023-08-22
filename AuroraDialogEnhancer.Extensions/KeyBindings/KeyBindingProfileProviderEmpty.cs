using System.Collections.Generic;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

internal class KeyBindingProfileProviderEmpty : IKeyBindingProfileProviderDto
{
    public KeyBindingProfileDto GetKeyBindingProfileDto() => new KeyBindingProfileDtoEmpty();

    public List<ClickablePointVmDto> GetClickablePointsVmDto() => new(0);
}
