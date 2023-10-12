using System;
using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

[Serializable]
public class AutoSkipConfig
{
    public List<List<GenericKey>> ActivationKeys { get; set; }

    public ESkipMode SkipMode { get; set; }

    public ESkipStartCondition StartCondition { get; set; }

    public List<GenericKey> SkipKeys { get; set; }

    public int Delay { get; set; }

    public bool IsDoubleClickDelay { get; set; }

    public int DoubleClickDelay { get; set; }

    public AutoSkipConfig(List<List<GenericKey>> activationKeys,
                          ESkipMode              skipMode,
                          ESkipStartCondition    startCondition,
                          List<GenericKey>       skipKeys,
                          int                    delay,
                          bool                   isDoubleClickDelay,
                          int                    doubleClickDelay)
    {
        ActivationKeys     = activationKeys;
        SkipMode           = skipMode;
        StartCondition     = startCondition;
        SkipKeys           = skipKeys;
        Delay              = delay;
        IsDoubleClickDelay = isDoubleClickDelay;
        DoubleClickDelay   = doubleClickDelay;
    }

    public AutoSkipConfig()
    {
        ActivationKeys = new List<List<GenericKey>>(0);
        SkipMode       = ESkipMode.Everything;
        StartCondition = ESkipStartCondition.Speaker;
        SkipKeys       = new List<GenericKey>(0);
    }
}
