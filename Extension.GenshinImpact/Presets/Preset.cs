using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.ClickablePoints;
using Extension.GenshinImpact.Services;

namespace Extension.GenshinImpact.Presets;

public class PresetBase : PresetBase
{
    public DialogOptionFinderProvider GetDialogOptionFinderProvider(Size clientSize) => new DialogOptionFinderInfoMapper().Map(clientSize);

    public List<ClickablePrecisePointDto> GetClickablePoints(Size clientSize) => new ClickableScreenPointsProvider().Get(clientSize);
}
