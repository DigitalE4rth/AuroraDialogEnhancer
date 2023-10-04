using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ClickablePoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

public class KeyBindingProfileViewModel
{
    #region Utilities
    public bool IsCursorHideOnManualClick { get; set; }
    public bool IsCycleThrough { get; set; }
    public ESingleDialogOptionBehaviour SingleDialogOptionBehaviour { get; set; }
    public ENumericActionBehaviour NumericActionBehaviour { get; set; }
    public ECursorBehaviour CursorBehaviour { get; set; }
    #endregion

    #region General
    public ActionViewModel PauseResume;
    public ActionViewModel Reload;
    public ActionViewModel Screenshot;
    public ActionViewModel HideCursor;
    #endregion

    #region Controls
    public ActionViewModel Select;
    public ActionViewModel Previous;
    public ActionViewModel Next;
    public Dictionary<string, ClickablePointVm> ClickablePoints;
    #endregion

    #region Scripts
    public AutoSkipViewModel AutoSkip;
    #endregion

    #region Numeric
    public ActionViewModel One;
    public ActionViewModel Two;
    public ActionViewModel Three;
    public ActionViewModel Four;
    public ActionViewModel Five;
    public ActionViewModel Six;
    public ActionViewModel Seven;
    public ActionViewModel Eight;
    public ActionViewModel Nine;
    public ActionViewModel Ten;
    #endregion

    public KeyBindingProfileViewModel()
    {
        #region Utilities
        IsCursorHideOnManualClick   = false;
        IsCycleThrough              = true;
        SingleDialogOptionBehaviour = ESingleDialogOptionBehaviour.Highlight;
        NumericActionBehaviour      = ENumericActionBehaviour.Highlight;
        CursorBehaviour             = ECursorBehaviour.Hide;
        #endregion

        #region General
        PauseResume = new ActionViewModel(new List<TriggerViewModel>());
        Reload      = new ActionViewModel(new List<TriggerViewModel>());
        Screenshot  = new ActionViewModel(new List<TriggerViewModel>());
        HideCursor  = new ActionViewModel(new List<TriggerViewModel>());
        #endregion

        #region Controls
        Select          = new ActionViewModel(new List<TriggerViewModel>());
        Previous        = new ActionViewModel(new List<TriggerViewModel>());
        Next            = new ActionViewModel(new List<TriggerViewModel>());
        ClickablePoints = new Dictionary<string, ClickablePointVm>();
        #endregion

        #region Scripts
        AutoSkip = new AutoSkipViewModel();
        #endregion

        #region Numeric
        One   = new ActionViewModel(new List<TriggerViewModel>());
        Two   = new ActionViewModel(new List<TriggerViewModel>());
        Three = new ActionViewModel(new List<TriggerViewModel>());
        Four  = new ActionViewModel(new List<TriggerViewModel>());
        Five  = new ActionViewModel(new List<TriggerViewModel>());
        Six   = new ActionViewModel(new List<TriggerViewModel>());
        Seven = new ActionViewModel(new List<TriggerViewModel>());
        Eight = new ActionViewModel(new List<TriggerViewModel>());
        Nine  = new ActionViewModel(new List<TriggerViewModel>());
        Ten   = new ActionViewModel(new List<TriggerViewModel>());
        #endregion
    }
}
