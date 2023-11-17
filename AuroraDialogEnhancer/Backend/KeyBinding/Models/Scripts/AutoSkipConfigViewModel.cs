using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

public class AutoSkipConfigViewModel
{
    public ActionViewModel ActivationKeys { get; set; }

    public ESkipMode SkipMode { get; set; }

    public ESkipStartCondition StartCondition { get; set; }

    public TriggerViewModel SkipKeys { get; set; }

    #region Text and replies / Text
    public int ScanDelayRegular { get; set; }

    public int ClickDelayRegular { get; set; }
    #endregion

    #region Replies
    public int ScanDelayReply { get; set; }

    public int ClickDelayReply { get; set; }
    #endregion

    public AutoSkipConfigViewModel(ActionViewModel     activationKeys,
                                   ESkipMode           skipMode,
                                   ESkipStartCondition startCondition,
                                   TriggerViewModel    skipKeys,
                                   int                 scanDelayRegular,
                                   int                 clickDelayRegular,
                                   int                 scanDelayReply,
                                   int                 clickDelayReply)
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

    public AutoSkipConfigViewModel(AutoSkipConfigViewModel viewModel)
    {
        ActivationKeys    = new ActionViewModel(viewModel.ActivationKeys);
        SkipMode          = (ESkipMode) viewModel.SkipMode;
        StartCondition    = (ESkipStartCondition) viewModel.StartCondition;
        SkipKeys          = new TriggerViewModel(viewModel.SkipKeys);
        ScanDelayRegular  = (int) viewModel.ScanDelayRegular;
        ClickDelayRegular = (int) viewModel.ClickDelayRegular;
        ScanDelayReply    = (int) viewModel.ScanDelayReply;
        ClickDelayReply   = (int) viewModel.ClickDelayReply;
    }

    public AutoSkipConfigViewModel()
    {
        ActivationKeys = new ActionViewModel(new List<TriggerViewModel>(0));
        SkipMode       = ESkipMode.Everything;
        StartCondition = ESkipStartCondition.Speaker;
        SkipKeys       = new TriggerViewModel();
    }
}
