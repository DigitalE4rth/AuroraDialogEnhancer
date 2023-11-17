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

    #region Text and replies / Text
    public int ScanDelayRegular { get; set; }

    public int ClickDelayRegular { get; set; }
    #endregion

    #region Reply
    public int ScanDelayReply { get; set; }

    public int ClickDelayReply { get; set; }
    #endregion

    public AutoSkipConfig(List<List<GenericKey>> activationKeys,
                          ESkipMode              skipMode,
                          ESkipStartCondition    startCondition,
                          List<GenericKey>       skipKeys,
                          int                    scanDelayRegular,
                          int                    clickDelayRegular,
                          int                    scanDelayReply,
                          int                    clickDelayReply)
    {
        ActivationKeys    = activationKeys;
        SkipMode          = skipMode;
        StartCondition    = startCondition;
        SkipKeys          = skipKeys;
        ScanDelayRegular  = scanDelayRegular;
        ClickDelayRegular = clickDelayRegular;
        ScanDelayReply    = scanDelayReply;
        ClickDelayReply   = clickDelayReply;
    }

    public AutoSkipConfig()
    {
        ActivationKeys = new List<List<GenericKey>>(0);
        SkipMode       = ESkipMode.Everything;
        StartCondition = ESkipStartCondition.Speaker;
        SkipKeys       = new List<GenericKey>(0);
    }
}
