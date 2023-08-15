namespace AuroraDialogEnhancerExtensions.KeyBindings;

public class MouseKeyDto : GenericKeyDto
{
    public MouseKeyDto()
    {
    }

    public MouseKeyDto(int keyCode) : base(keyCode)
    {
    }

    public MouseKeyDto(EHighMouseKeyDto highMouseKeyDto) : base((int) highMouseKeyDto)
    {
    }
}
