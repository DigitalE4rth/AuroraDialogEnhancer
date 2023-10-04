using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;

public class MouseKeyInterpreterService
{
    public string GetAsString(EHighMouseKey mouseKey)
    {
        return mouseKey switch
        {
            EHighMouseKey.MouseWheelDown => "Mouse Wheel Down",
            EHighMouseKey.MouseWheelUp   => "Mouse Wheel Up",
            EHighMouseKey.MiddleButton   => "Middle Button",
            EHighMouseKey.Back           => "Back",
            EHighMouseKey.Forward        => "Forward",
            _                            => "Unknown"
        };
    }
}
