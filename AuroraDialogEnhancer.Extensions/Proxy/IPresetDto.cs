using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace AuroraDialogEnhancerExtensions.Proxy;

public interface IPresetDto
{
    public DialogOptionFinderInfo DialogOptionsFinderInfo { get; }

    public List<ClickableScreenPointDto> ClickablePoints { get; }
}
