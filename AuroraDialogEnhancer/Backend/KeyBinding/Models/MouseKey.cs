namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

public record MouseKey : GenericKey
{
    public MouseKey(int keyCode) : base(keyCode)
    {
    }

    public MouseKey(EHighMouseKey highMouseKey) : base((int) highMouseKey)
    {
    }

    public MouseKey()
    {
    }
}
