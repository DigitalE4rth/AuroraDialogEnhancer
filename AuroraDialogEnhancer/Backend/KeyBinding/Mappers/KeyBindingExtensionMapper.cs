using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ClickablePoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
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

            #region Scripts
            AutoSkip = new AutoSkip
            {
                Id               = obj.AutoSkipDto.Id,
                ActivationKeys   = Map(obj.AutoSkipDto.ActivationKeys),
                AutoSkipType     = (EAutoSkipType) obj.AutoSkipDto.AutoSkipType,
                SkipKeys         = Map(obj.AutoSkipDto.SkipKeys),
                Delay            = obj.AutoSkipDto.Delay,
                DoubleClickDelay = obj.AutoSkipDto.DoubleClickDelay
            },
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
        return genericKeyExtList.Select(Map).ToList();
    }

    private GenericKey Map(GenericKeyDto genericKeyExt)
    {
        return genericKeyExt.GetType() == typeof(KeyboardKeyDto)
            ? new KeyboardKey(genericKeyExt.KeyCode)
            : new MouseKey(genericKeyExt.KeyCode);
    }

    private List<ClickablePoint> Map(IEnumerable<ClickablePointDto> listOfClickablePoints)
    {
        return listOfClickablePoints.Select(pointDto => new ClickablePoint(pointDto.Id, Map(pointDto.Keys))).ToList();
    }
}
