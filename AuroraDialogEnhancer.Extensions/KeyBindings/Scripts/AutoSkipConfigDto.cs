using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings.Keys;

namespace AuroraDialogEnhancerExtensions.KeyBindings.Scripts;

public class AutoSkipConfigDto
{
    public List<List<GenericKeyDto>> ActivationKeys { get; set; }

    public ESkipModeDto SkipMode { get; set; }

    public ESkipStartConditionDto StartCondition { get; set; }

    public List<GenericKeyDto> SkipKeys { get; set; }

    public int Delay { get; set; }

    public bool IsDoubleClickDelay { get; set; }

    public int DoubleClickDelay { get; set; }

    public AutoSkipConfigDto(List<List<GenericKeyDto>> activationKeys,
                             ESkipModeDto              skipMode,
                             ESkipStartConditionDto    startCondition,
                             List<GenericKeyDto>       skipKeys,
                             int                       delay,
                             bool                      isDoubleClickDelay,
                             int                       doubleClickDelay)
    {
        ActivationKeys     = activationKeys;
        SkipMode           = skipMode;
        StartCondition     = startCondition;
        SkipKeys           = skipKeys;
        Delay              = delay;
        IsDoubleClickDelay = isDoubleClickDelay;
        DoubleClickDelay   = doubleClickDelay;
    }

    public AutoSkipConfigDto()
    {
        ActivationKeys = new List<List<GenericKeyDto>>(0);
        SkipMode       = ESkipModeDto.Everything;
        StartCondition = ESkipStartConditionDto.Speaker;
        SkipKeys       = new List<GenericKeyDto> { new KeyboardKeyDto(32) };
    }
}
