using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;
using AuroraDialogEnhancerExtensions.Screenshots;

namespace AuroraDialogEnhancerExtensions.Proxy;

public abstract class PresetBase
{
    public virtual IScreenshotNameProvider GetScreenshotNameProvider() => new ScreenshotNameProviderDefault();

    public virtual DialogOptionFinderProvider GetDialogOptionFinderProvider(Size clientSize)
    {
        return new DialogOptionFinderProvider(
            new DialogOptionFinderEmpty(),
            new DialogOptionFinderData(Rectangle.Empty, Rectangle.Empty,
                new CursorPositionData(), 0.02));
    }

    public virtual List<InteractionPrecisePointDto> GetInteractionPoints(Size clientSize) => new(0);
}
