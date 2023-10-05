using System;
using System.Collections.Generic;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

public class AutoSkipViewModel
{
    public string Id { get; set; }

    public ActionViewModel ActivationKeys { get; set; }

    public EAutoSkipType AutoSkipType { get; set; }

    public TriggerViewModel SkipKeys { get; set; }

    public int Delay { get; set; }

    public int DoubleClickDelay { get; set; }


    public AutoSkipViewModel(string id,
                             ActionViewModel  activationKeys,
                             EAutoSkipType    autoSkipType,
                             TriggerViewModel skipKeys,
                             int              delay,
                             int              doubleClickDelay)
    {
        Id               = id;
        ActivationKeys   = activationKeys;
        AutoSkipType     = autoSkipType;
        SkipKeys         = skipKeys;
        Delay            = delay;
        DoubleClickDelay = doubleClickDelay;
    }

    public AutoSkipViewModel()
    {
        Id             = Guid.NewGuid().ToString();
        ActivationKeys = new ActionViewModel(new List<TriggerViewModel>(0));
        AutoSkipType   = EAutoSkipType.Partial;
        SkipKeys       = new TriggerViewModel();
    }
}
