using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Mappers;

public class KeyBindingExtensionMapper : IMapper<KeyBindingProfileDto, KeyBindingProfile>
{
    public KeyBindingProfile Map(KeyBindingProfileDto obj)
    {
        var profile = new KeyBindingProfile
        {
            #region Utilities
            IsCursorHideOnManualClick   = obj.IsCursorHideOnManualClick,
            IsCycleThrough              = obj.IsCycleThrough,
            SingleDialogOptionBehaviour = (ESingleDialogOptionBehaviour) obj.SingleDialogOptionBehaviourDto,
            NumericActionBehaviour      = (ENumericActionBehaviour)      obj.NumericActionBehaviourDto,
            CursorBehaviour             = (ECursorBehaviour)             obj.CursorBehaviourDto,
            HiddenCursorSetting         = (EHiddenCursorSetting)         obj.HiddenCursorSettingDto,
            #endregion

            #region General
            PauseResume = Map(obj.PauseResume),
            Reload      = Map(obj.Reload),
            Screenshot  = Map(obj.Screenshot),
            HideCursor  = Map(obj.HideCursor),
            #endregion

            #region Controls
            Select          = Map(obj.Select),
            Previous        = Map(obj.Previous),
            Next            = Map(obj.Next),
            ClickablePoints = Map(obj.ClickablePoints),
            #endregion

            #region Controls
            One   = Map(obj.One),
            Two   = Map(obj.Two),
            Three = Map(obj.Three),
            Four  = Map(obj.Four),
            Five  = Map(obj.Five),
            Six   = Map(obj.Six),
            Seven = Map(obj.Seven),
            Eight = Map(obj.Eight),
            Nine  = Map(obj.Nine),
            Ten   = Map(obj.Ten)
            #endregion
        };


        return profile;
    }

    private List<List<GenericKey>> Map(IEnumerable<List<GenericKeyDto>> genericKeyExtList)
    {
        return genericKeyExtList.Select(Map).ToList();
    }

    private List<GenericKey> Map(List<GenericKeyDto> genericKeyExtList)
    {
        var result = new List<GenericKey>();
        foreach (var genericKeyExt in genericKeyExtList)
        {
            if (genericKeyExt.GetType() == typeof(KeyboardKeyDto))
            {
                result.Add(new KeyboardKey(genericKeyExt.KeyCode));
                continue;
            }
            result.Add(new MouseKey(genericKeyExt.KeyCode));
        }

        return result;
    }

    private List<ClickablePoint> Map(IEnumerable<ClickablePointDto> listOfClickablePoints)
    {
        return listOfClickablePoints.Select(pointDto => new ClickablePoint(pointDto.Id, Map(pointDto.Keys))).ToList();
    }
}
