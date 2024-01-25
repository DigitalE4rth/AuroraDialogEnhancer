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
        return new DialogOptionFinderProvider(new DialogOptionFinderEmpty(), new PresetData());
    }

    public virtual List<InteractionPrecisePointDto> GetInteractionPoints(Size clientSize) => new(0);
}
