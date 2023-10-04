using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

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
