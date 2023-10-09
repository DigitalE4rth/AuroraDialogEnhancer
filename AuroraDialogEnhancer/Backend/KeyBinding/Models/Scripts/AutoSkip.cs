using System;
using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

[Serializable]
public class AutoSkip
{
    public List<List<GenericKey>> ActivationKeys { get; set; }

    public EAutoSkipType AutoSkipType { get; set; }

    public List<GenericKey> SkipKeys { get; set; }

    public int Delay { get; set; }

    public bool IsDoubleClickDelay { get; set; }

    public int DoubleClickDelay { get; set; }

    public AutoSkip(List<List<GenericKey>> activationKeys,
                    EAutoSkipType          autoSkipType,
                    List<GenericKey>       skipKeys,
                    int                    delay,
                    bool                   isDoubleClickDelay,
                    int                    doubleClickDelay)
    {
        ActivationKeys     = activationKeys;
        AutoSkipType       = autoSkipType;
        SkipKeys           = skipKeys;
        Delay              = delay;
        IsDoubleClickDelay = isDoubleClickDelay;
        DoubleClickDelay   = doubleClickDelay;
    }

    public AutoSkip()
    {
        ActivationKeys = new List<List<GenericKey>>(0);
        AutoSkipType   = EAutoSkipType.Everything;
        SkipKeys       = new List<GenericKey>(0);
    }
}
