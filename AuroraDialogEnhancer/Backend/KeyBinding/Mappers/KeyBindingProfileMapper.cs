using System.Linq;
using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancerExtensions.KeyBinding;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Mappers;

public class KeyBindingProfileMapper : IMapper<KeyBindingProfileDto,       KeyBindingProfile>,
                                       IMapper<KeyBindingProfileViewModel, KeyBindingProfile>
{
    public KeyBindingProfile Map(KeyBindingProfileDto obj)
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
            PauseResume = obj.PauseResume,
            Reload      = obj.Reload,
            Screenshot  = obj.Screenshot,
            HideCursor  = obj.HideCursor,
            #endregion

            #region Controls
            Select          = obj.Select,
            Previous        = obj.Previous,
            Next            = obj.Next,
            AutoDialog      = obj.AutoDialog,
            HideUi          = obj.HideUi,
            FullScreenPopUp = obj.FullScreenPopUp,
            #endregion

            #region Numeric
            One   = obj.One,
            Two   = obj.Two,
            Three = obj.Three,
            Four  = obj.Four,
            Five  = obj.Five,
            Six   = obj.Six,
            Seven = obj.Seven,
            Eight = obj.Eight,
            Nine  = obj.Nine,
            Ten   = obj.Ten
            #endregion
        };
    }

    public KeyBindingProfile Map(KeyBindingProfileViewModel obj)
    {
        return new KeyBindingProfile
        {
            #region Utilities
            IsCursorHideOnManualClick = obj.IsCursorHideOnManualClick,
            IsCycleThrough = obj.IsCycleThrough,
            SingleDialogOptionBehaviour = obj.SingleDialogOptionBehaviour,
            NumericActionBehaviour = obj.NumericActionBehaviour,
            CursorBehaviour = obj.CursorBehaviour,
            HiddenCursorSetting = obj.HiddenCursorSetting,
            #endregion

            #region General
            PauseResume = obj.PauseResume.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Reload = obj.Reload.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Screenshot = obj.Screenshot.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            HideCursor = obj.HideCursor.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            #endregion

            #region Controls
            Select = obj.Select.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Previous = obj.Previous.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Next = obj.Next.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            AutoDialog = obj.AutoDialog.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            HideUi = obj.HideUi.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            FullScreenPopUp = obj.FullScreenPopUp.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            #endregion

            #region Numeric
            One = obj.One.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Two = obj.Two.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Three = obj.Three.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Four = obj.Four.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Five = obj.Five.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Six = obj.Six.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Seven = obj.Seven.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Eight = obj.Eight.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Nine = obj.Nine.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList(),
            Ten = obj.Ten.TriggerViewModels.Select(tvm => tvm.KeyCodes).ToList()
            #endregion
        };
    }
}
