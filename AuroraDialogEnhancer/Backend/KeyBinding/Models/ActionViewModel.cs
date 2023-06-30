using System.Collections.Generic;
using System.Linq;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

public class ActionViewModel
{
    public List<TriggerViewModel> TriggerViewModels { get; set; }

    public ActionViewModel(List<TriggerViewModel> triggerViewModels)
    {
        TriggerViewModels = triggerViewModels;
    }

    public ActionViewModel(ActionViewModel actionViewModel)
    {
        TriggerViewModels = actionViewModel.TriggerViewModels.Select(tvm => new TriggerViewModel(tvm)).ToList();
    }
}
