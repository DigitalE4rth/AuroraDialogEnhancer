using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings.Keys;

namespace AuroraDialogEnhancerExtensions.KeyBindings.Scripts;

public class AutoSkipConfigDto
{
    #region General
    public List<List<GenericKeyDto>> ActivationKeys { get; set; }

    public ESkipModeDto SkipMode { get; set; }

    public ESkipStartConditionDto StartCondition { get; set; }

    public List<GenericKeyDto> SkipKeys { get; set; }
    #endregion

    #region Text and replies / Text
    public int ScanDelayRegular { get; set; }

    public int ClickDelayRegular { get; set; }
    #endregion

    #region Reply
    public int ScanDelayReply { get; set; }

    public int ClickDelayReply { get; set; }
    #endregion

    public AutoSkipConfigDto(List<List<GenericKeyDto>> activationKeys,
                             ESkipModeDto              skipMode,
                             ESkipStartConditionDto    startCondition,
                             List<GenericKeyDto>       skipKeys,
                             int                       scanDelayRegular,
                             int                       clickDelayRegular,
                             int                       scanDelayReply,
                             int                       clickDelayReply)
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

    public AutoSkipConfigDto()
    {
        ActivationKeys = new List<List<GenericKeyDto>>(0);
        SkipMode       = ESkipModeDto.Everything;
        StartCondition = ESkipStartConditionDto.Speaker;
        SkipKeys       = new List<GenericKeyDto> { new KeyboardKeyDto(32) };
    }
}
