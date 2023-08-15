using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.ClickablePoints;
using Extension.GenshinImpact.Services;

namespace Extension.GenshinImpact.Presets;

public class Preset : IPresetDto
{
    public DialogOptionFinderInfo DialogOptionsFinderInfo { get; }

    public List<ClickableScreenPointDto> ClickablePoints { get; }

    public Preset(Size clientSize)
    {
        DialogOptionsFinderInfo = new DialogOptionFinderInfoMapper().Map(clientSize);
        ClickablePoints = new ClickableScreenPointsProvider().Get(clientSize);
    }
}