using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace AuroraDialogEnhancerExtensions.Proxy;

internal class PresetEmpty : IPreset
{
    public DialogOptionFinderProvider GetDialogOptionFinderProvider(Size clientSize)
    {
        return new DialogOptionFinderProvider(
            new DialogOptionFinderEmpty(),
            new DialogOptionFinderData(Rectangle.Empty, Rectangle.Empty, Point.Empty));
    }

    public List<ClickablePrecisePoint> GetClickablePoints(Size clientSize) => new(0);
}
