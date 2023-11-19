using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Mappers;

public class KeyBindingViewModelBackMapper : IMapper<KeyBindingProfileViewModel, KeyBindingProfile>
{
    public KeyBindingProfile Map(KeyBindingProfileViewModel obj)
    {
        return new KeyBindingProfile
        {
            #region Utilities
            IsCursorHideOnManualClick   = obj.IsCursorHideOnManualClick,
            IsCycleThrough              = obj.IsCycleThrough,
            SingleDialogOptionBehaviour = obj.SingleDialogOptionBehaviour,
            NumericActionBehaviour      = obj.NumericActionBehaviour,
            CursorBehaviour             = obj.CursorBehaviour,
            #endregion

            #region General
            PauseResume = Map(obj.PauseResume),
            Reload      = Map(obj.Reload),
            Screenshot  = Map(obj.Screenshot),
            HideCursor  = Map(obj.HideCursor),
            #endregion

            #region Controls
            Select   = Map(obj.Select),
            Previous = Map(obj.Previous),
            Next     = Map(obj.Next),
            #endregion

            #region Interaction Points
            InteractionPoints = Map(obj.InteractionPoints),
            #endregion

            #region Scripts
            AutoSkipConfig = new AutoSkipConfig 
            {
                ActivationKeys    = Map(obj.AutoSkipConfig.ActivationKeys),
                SkipMode          = obj.AutoSkipConfig.SkipMode,
                StartCondition    = obj.AutoSkipConfig.StartCondition,
                SkipKeys          = Map(obj.AutoSkipConfig.SkipKeys),
                ScanDelayRegular  = obj.AutoSkipConfig.ScanDelayRegular,
                ClickDelayRegular = obj.AutoSkipConfig.ClickDelayRegular,
                ScanDelayReply    = obj.AutoSkipConfig.ScanDelayReply,
                ClickDelayReply   = obj.AutoSkipConfig.ClickDelayReply
            },
            #endregion

            #region Numeric
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
    }

    private List<List<GenericKey>> Map(ActionViewModel actionViewModel)
    {
        return actionViewModel.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList();
    }

    private List<GenericKey> Map(TriggerViewModel triggerViewModel)
    {
        return triggerViewModel.KeyCodes.ToList();
    }

    private List<InteractionPoint> Map(Dictionary<string, InteractionPointVm> interactionPointsVm)
    {
        return interactionPointsVm.Select(vm => new InteractionPoint(vm.Value.Id, Map(vm.Value.ActionViewModel))).ToList();
    }
}
