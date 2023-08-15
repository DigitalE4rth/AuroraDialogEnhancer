using System.Collections.Generic;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

public abstract class KeyBindingProfileProviderDto
{
    public virtual KeyBindingProfileDto KeyBindingProfileDto { get; } = new KeyBindingProfileDtoEmpty();

    public virtual List<ClickablePointVmDto> ClickablePointsVm { get; } = new();

    protected KeyBindingProfileProviderDto(KeyBindingProfileDto keyBindingProfileDto, List<ClickablePointVmDto> clickablePointsVm)
    {
        KeyBindingProfileDto = keyBindingProfileDto;
        ClickablePointsVm = clickablePointsVm;
    }
    
    protected KeyBindingProfileProviderDto()
    {
    }
}
