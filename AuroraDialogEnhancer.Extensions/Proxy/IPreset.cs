using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace AuroraDialogEnhancerExtensions.Proxy;

public interface IPreset
{
    public DialogOptionFinderProvider? GetDialogOptionFinderProvider(Size clientSize);

    public List<ClickablePrecisePointDto>? GetClickablePoints(Size clientSize);
}
