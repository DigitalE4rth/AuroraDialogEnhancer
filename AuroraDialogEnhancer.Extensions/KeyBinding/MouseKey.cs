namespace AuroraDialogEnhancerExtensions.KeyBinding;

public class MouseKey : GenericKey
{
    public MouseKey()
    {
    }

    public MouseKey(int keyCode) : base(keyCode)
    {
    }

    public MouseKey(EHighMouseKey highMouseKey) : base((int) highMouseKey)
    {
    }
}
