using System;
using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

[Serializable]
public class AutoSkip
{
    public string Id { get; set; }

    public List<List<GenericKey>> ActivationKeys { get; set; }

    public EAutoSkipType AutoSkipType { get; set; }

    public List<GenericKey> SkipKeys { get; set; }

    public int Delay { get; set; }

    public bool IsDoubleClickRequired { get; set; }

    public AutoSkip(string                 id,
                    List<List<GenericKey>> activationKeys,
                    EAutoSkipType          autoSkipType,
                    List<GenericKey>       skipKeys,
                    int                    delay,
                    bool                   isDoubleClickRequired)
    {
        Id                    = id;
        ActivationKeys        = activationKeys;
        AutoSkipType          = autoSkipType;
        SkipKeys              = skipKeys;
        Delay                 = delay;
        IsDoubleClickRequired = isDoubleClickRequired;
    }

    public AutoSkip()
    {
        Id                    = Guid.NewGuid().ToString();
        ActivationKeys        = new List<List<GenericKey>>(0);
        AutoSkipType          = EAutoSkipType.Partial;
        SkipKeys              = new List<GenericKey>(0);
        Delay                 = 0;
        IsDoubleClickRequired = false;
    }
}
