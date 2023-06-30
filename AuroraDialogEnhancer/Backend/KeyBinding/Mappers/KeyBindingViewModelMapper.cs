using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancerExtensions.KeyBinding;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Mappers;

public class KeyBindingViewModelMapper : IMapper<KeyBindingProfileDto, KeyBindingProfileViewModel>
{
    private readonly KeyInterpreterService _keyInterpreterService;

    public KeyBindingViewModelMapper(KeyInterpreterService keyInterpreterService)
    {
        _keyInterpreterService = keyInterpreterService;
    }

    public KeyBindingProfileViewModel Map(KeyBindingProfileDto obj)
    {
        return new KeyBindingProfileViewModel
        {
            IsCursorHideOnManualClick   = obj.IsCursorHideOnManualClick,
            IsCycleThrough              = obj.IsCycleThrough,
            SingleDialogOptionBehaviour = obj.SingleDialogOptionBehaviour,
            NumericActionBehaviour      = obj.NumericActionBehaviour,
            CursorBehaviour             = obj.CursorBehaviour,
            HiddenCursorSetting         = obj.HiddenCursorSetting,

            Reload      = GetActionViewModel(obj.Reload),
            PauseResume = GetActionViewModel(obj.PauseResume),
            Screenshot  = GetActionViewModel(obj.Screenshot),
            HideCursor  = GetActionViewModel(obj.HideCursor),

            Previous        = GetActionViewModel(obj.Previous),
            Select          = GetActionViewModel(obj.Select),
            Next            = GetActionViewModel(obj.Next),
            AutoDialog      = GetActionViewModel(obj.AutoDialog),
            HideUi          = GetActionViewModel(obj.HideUi),
            FullScreenPopUp = GetActionViewModel(obj.FullScreenPopUp),

            One   = GetActionViewModel(obj.One),
            Two   = GetActionViewModel(obj.Two),
            Three = GetActionViewModel(obj.Three),
            Four  = GetActionViewModel(obj.Four),
            Five  = GetActionViewModel(obj.Five),
            Six   = GetActionViewModel(obj.Six),
            Seven = GetActionViewModel(obj.Seven),
            Eight = GetActionViewModel(obj.Eight),
            Nine  = GetActionViewModel(obj.Nine),
            Ten   = GetActionViewModel(obj.Ten)
        };
    }

    private ActionViewModel GetActionViewModel(IEnumerable<List<GenericKey>> lowActions)
    {
        return new ActionViewModel(lowActions.Select(rawKeys => new TriggerViewModel(rawKeys, _keyInterpreterService.InterpretKeys(rawKeys))).ToList());
    }
}
