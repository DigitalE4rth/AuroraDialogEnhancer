using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace Extension.GenshinImpact.KeyBindings;

public class KeyBindingProfileProvider : IKeyBindingProfileProviderDto
{
    public KeyBindingProfileDto GetKeyBindingProfileDto() => new KeyBindingProfile();

    public List<ClickablePointVmDto> GetClickablePointsVmDto() => new ClickablePointViewModelProvider().Get();
}
