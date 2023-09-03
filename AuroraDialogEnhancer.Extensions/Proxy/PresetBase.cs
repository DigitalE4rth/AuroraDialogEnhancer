using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.Screenshots;

namespace AuroraDialogEnhancerExtensions.Proxy;

public abstract class PresetBase
{
    public virtual IScreenshotNameProvider GetScreenshotNameProvider() => new ScreenshotNameProviderDefault();

    public virtual DialogOptionFinderProvider? GetDialogOptionFinderProvider(Size clientSize)
    {
        return new DialogOptionFinderProvider(
            new DialogOptionFinderEmpty(),
            new DialogOptionFinderData(Rectangle.Empty, Rectangle.Empty, Point.Empty));
    }

    public virtual List<ClickablePrecisePointDto>? GetClickablePoints(Size clientSize) => new(0);
}
