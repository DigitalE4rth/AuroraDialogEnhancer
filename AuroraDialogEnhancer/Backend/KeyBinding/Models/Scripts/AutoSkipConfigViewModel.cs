﻿using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

public class AutoSkipConfigViewModel
{
    public ActionViewModel ActivationKeys { get; set; }

    public EAutoSkipType AutoSkipType { get; set; }

    public TriggerViewModel SkipKeys { get; set; }

    public int Delay { get; set; }

    public bool IsDoubleClickDelay { get; set; }

    public int DoubleClickDelay { get; set; }

    public AutoSkipConfigViewModel(ActionViewModel  activationKeys,
                                   EAutoSkipType    autoSkipType,
                                   TriggerViewModel skipKeys,
                                   int              delay,
                                   bool             isDoubleClickDelay,
                                   int              doubleClickDelay)
    {
        ActivationKeys     = activationKeys;
        AutoSkipType       = autoSkipType;
        SkipKeys           = skipKeys;
        Delay              = delay;
        IsDoubleClickDelay = isDoubleClickDelay;
        DoubleClickDelay   = doubleClickDelay;
    }

    public AutoSkipConfigViewModel(AutoSkipConfigViewModel viewModel)
    {
        ActivationKeys     = new ActionViewModel(viewModel.ActivationKeys);
        AutoSkipType       = (EAutoSkipType) viewModel.AutoSkipType;
        SkipKeys           = new TriggerViewModel(viewModel.SkipKeys);
        Delay              = (int) viewModel.Delay;
        IsDoubleClickDelay = (bool) viewModel.IsDoubleClickDelay;
        DoubleClickDelay   = (int) viewModel.DoubleClickDelay;
    }

    public AutoSkipConfigViewModel()
    {
        ActivationKeys = new ActionViewModel(new List<TriggerViewModel>(0));
        AutoSkipType   = EAutoSkipType.Partial;
        SkipKeys       = new TriggerViewModel();
    }
}
