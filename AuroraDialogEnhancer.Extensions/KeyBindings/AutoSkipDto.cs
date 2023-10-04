using System;
using System.Collections.Generic;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

public class AutoSkipDto
{
    public string Id { get; set; }

    public List<List<GenericKeyDto>> ActivationKeys { get; set; }

    public EAutoSkipTypeDto AutoSkipType { get; set; }

    public List<GenericKeyDto> SkipKeys { get; set; }

    public int Delay { get; set; }

    public bool IsDoubleClickRequired { get; set; }

    public AutoSkipDto(string id,
                       List<List<GenericKeyDto>> activationKeys,
                       EAutoSkipTypeDto          autoSkipType,
                       List<GenericKeyDto>       skipKeys,
                       int                       delay,
                       bool                      isDoubleClickRequired)
    {
        Id                    = id;
        ActivationKeys        = activationKeys;
        AutoSkipType          = autoSkipType;
        SkipKeys              = skipKeys;
        Delay                 = delay;
        IsDoubleClickRequired = isDoubleClickRequired;
    }

    public AutoSkipDto()
    {
        Id                    = Guid.NewGuid().ToString();
        ActivationKeys        = new List<List<GenericKeyDto>>(0);
        AutoSkipType          = EAutoSkipTypeDto.Partial;
        SkipKeys              = new List<GenericKeyDto> { new KeyboardKeyDto(32) };
        Delay                 = 0;
        IsDoubleClickRequired = false;
    }
}
