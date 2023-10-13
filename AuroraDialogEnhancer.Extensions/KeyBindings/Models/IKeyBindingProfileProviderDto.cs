using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings.ClickablePoints;

namespace AuroraDialogEnhancerExtensions.KeyBindings.Models;

public interface IKeyBindingProfileProviderDto
{
    public KeyBindingProfileDto GetKeyBindingProfileDto();

    public List<ClickablePointVmDto> GetClickablePointsVmDto();
}
