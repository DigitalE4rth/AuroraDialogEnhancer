using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.KeyBindings.ClickablePoints;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.ClickablePoints;
using Extension.GenshinImpact.Services;

namespace Extension.GenshinImpact.Presets;

public class Preset : PresetBase
{
    public override DialogOptionFinderProvider GetDialogOptionFinderProvider(Size clientSize) => new DialogOptionFinderInfoMapper().Map(clientSize);

    public override List<ClickablePrecisePointDto> GetClickablePoints(Size clientSize) => new ClickableScreenPointsProvider().Get(clientSize);
}
