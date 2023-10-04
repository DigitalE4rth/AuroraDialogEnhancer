namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

public record KeyboardKey : GenericKey
{
    public KeyboardKey(int keyCode) : base(keyCode)
    {
    }

    public KeyboardKey()
    {
    }
}
