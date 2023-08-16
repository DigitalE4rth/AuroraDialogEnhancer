﻿using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;

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
            HiddenCursorSetting         = obj.HiddenCursorSetting,
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

    private List<ClickablePoint> Map(Dictionary<string, ClickablePointVm> clickablePointsVm)
    {
        return clickablePointsVm.Select(vm => new ClickablePoint(vm.Value.Id, Map(vm.Value.ActionViewModel))).ToList();
    }
}