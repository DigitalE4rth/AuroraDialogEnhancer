namespace AuroraDialogEnhancerExtensions.KeyBindings;

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
