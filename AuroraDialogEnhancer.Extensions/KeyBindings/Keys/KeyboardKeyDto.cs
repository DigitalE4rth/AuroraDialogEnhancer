namespace AuroraDialogEnhancerExtensions.KeyBindings.Keys;

public record KeyboardKeyDto : GenericKeyDto
{
    public KeyboardKeyDto(int keyCode) : base(keyCode)
    {
    }

    public KeyboardKeyDto()
    {
    }
}
