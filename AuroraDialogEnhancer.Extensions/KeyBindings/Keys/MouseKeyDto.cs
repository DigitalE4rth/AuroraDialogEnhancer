using AuroraDialogEnhancerExtensions.KeyBindings.Behaviour;

namespace AuroraDialogEnhancerExtensions.KeyBindings.Keys;

public record MouseKeyDto : GenericKeyDto
{
    public MouseKeyDto(EHighMouseKeyDto highMouseKeyDto) : base((int)highMouseKeyDto)
    {
    }

    public MouseKeyDto(int keyCode) : base(keyCode)
    {
    }

    public MouseKeyDto()
    {
    }
}
