using System;
using System.Collections.Generic;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

public class AutoSkipDto
{
    public List<List<GenericKeyDto>> ActivationKeys { get; set; }

    public EAutoSkipTypeDto AutoSkipType { get; set; }

    public List<GenericKeyDto> SkipKeys { get; set; }

    public int Delay { get; set; }

    public bool IsDoubleClickDelay { get; set; }

    public int DoubleClickDelay { get; set; }

    public AutoSkipDto(List<List<GenericKeyDto>> activationKeys,
                       EAutoSkipTypeDto          autoSkipType,
                       List<GenericKeyDto>       skipKeys,
                       int                       delay,
                       bool                      isDoubleClickDelay,
                       int                       doubleClickDelay)
    {
        ActivationKeys     = activationKeys;
        AutoSkipType       = autoSkipType;
        SkipKeys           = skipKeys;
        Delay              = delay;
        IsDoubleClickDelay = isDoubleClickDelay;
        DoubleClickDelay   = doubleClickDelay;
    }

    public AutoSkipDto()
    {
        ActivationKeys = new List<List<GenericKeyDto>>(0);
        AutoSkipType   = EAutoSkipTypeDto.Everything;
        SkipKeys       = new List<GenericKeyDto> { new KeyboardKeyDto(32) };
    }
}
