using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.InteractionPoints;
using Extension.GenshinImpact.Services;

namespace Extension.GenshinImpact.Presets;

public class Preset : PresetBase
{
    public override DialogOptionFinderProvider GetDialogOptionFinderProvider(Size clientSize) 
        => new DialogOptionFinderInfoMapper().Map(clientSize);

    public override List<InteractionPrecisePointDto> GetInteractionPoints(Size clientSize) 
        => new InteractionScreenPointsProvider().Get(clientSize);
}
