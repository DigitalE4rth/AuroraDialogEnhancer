using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

public class KeyBindingProfileViewModel
{
    #region Utilities
    public bool IsCursorHideOnManualClick { get; set; } = false;
    public bool IsCycleThrough            { get; set; } = true;
    public ESingleDialogOptionBehaviour SingleDialogOptionBehaviour { get; set; } = ESingleDialogOptionBehaviour.Highlight;
    public ENumericActionBehaviour      NumericActionBehaviour      { get; set; } = ENumericActionBehaviour.Highlight;
    public ECursorBehaviour             CursorBehaviour             { get; set; } = ECursorBehaviour.Hide;
    #endregion

    #region General
    public ActionViewModel PauseResume = new(new List<TriggerViewModel>());
    public ActionViewModel Reload      = new(new List<TriggerViewModel>());
    public ActionViewModel Screenshot  = new(new List<TriggerViewModel>());
    public ActionViewModel HideCursor  = new(new List<TriggerViewModel>());
    #endregion

    #region Controls
    public ActionViewModel Select   = new(new List<TriggerViewModel>());
    public ActionViewModel Previous = new(new List<TriggerViewModel>());
    public ActionViewModel Next     = new(new List<TriggerViewModel>());
    public ActionViewModel Last     = new(new List<TriggerViewModel>());
    #endregion

    #region Interaction Points
    public Dictionary<string, InteractionPointVm> InteractionPoints = new();
    #endregion

    #region Scripts
    public AutoSkipConfigViewModel AutoSkipConfig = new();
    #endregion

    #region Numeric
    public ActionViewModel One   = new(new List<TriggerViewModel>());
    public ActionViewModel Two   = new(new List<TriggerViewModel>());
    public ActionViewModel Three = new(new List<TriggerViewModel>());
    public ActionViewModel Four  = new(new List<TriggerViewModel>());
    public ActionViewModel Five  = new(new List<TriggerViewModel>());
    public ActionViewModel Six   = new(new List<TriggerViewModel>());
    public ActionViewModel Seven = new(new List<TriggerViewModel>());
    public ActionViewModel Eight = new(new List<TriggerViewModel>());
    public ActionViewModel Nine  = new(new List<TriggerViewModel>());
    public ActionViewModel Ten   = new(new List<TriggerViewModel>());
    #endregion
}
