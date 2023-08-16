using System.Collections.Generic;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

public interface IKeyBindingProfileProviderDto
{
    public KeyBindingProfileDto GetKeyBindingProfileDto();

    public List<ClickablePointVmDto> GetClickablePointsVm();
}
