using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;
using AuroraDialogEnhancerExtensions.Proxy;
using AuroraDialogEnhancerExtensions.Screenshots;
using Extension.HonkaiStarRail.InteractionPoints;
using Extension.HonkaiStarRail.Screenshots;
using Extension.HonkaiStarRail.Services;

namespace Extension.HonkaiStarRail.Presets;

public class Preset : PresetBase
{
    public override IScreenshotNameProvider GetScreenshotNameProvider() => new ScreenshotNameProvider();

    public override DialogOptionFinderProvider GetDialogOptionFinderProvider(Size clientSize) => new DialogOptionFinderInfoMapper().Map(clientSize);

    public override List<InteractionPrecisePointDto> GetInteractionPoints(Size clientSize) => new InteractionScreenPointsProvider().Get(clientSize);
}
