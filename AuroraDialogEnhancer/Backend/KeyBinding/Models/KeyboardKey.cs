namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

public record KeyboardKey : GenericKey
{
    public KeyboardKey(int keyCode) : base(keyCode)
    {
    }

    public KeyboardKey()
    {
    }
}
