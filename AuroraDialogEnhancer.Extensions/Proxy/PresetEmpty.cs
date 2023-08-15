using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace AuroraDialogEnhancerExtensions.Proxy;

internal class PresetEmpty : IPresetDto
{
    public DialogOptionFinderInfo DialogOptionsFinderInfo { get; } = new(Rectangle.Empty, Rectangle.Empty, new DialogOptionFinderEmpty());

    public List<ClickableScreenPointDto> ClickablePoints { get; } = new(0);
}
