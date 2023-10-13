using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

public class TriggerViewModel
{
    public List<GenericKey> KeyCodes { get; set; }

    public List<string> KeyNames { get; set; }

    public TriggerViewModel(List<GenericKey> rawKeyCodes, List<string> keyNames)
    {
        KeyCodes = rawKeyCodes;
        KeyNames = keyNames;
    }

    public TriggerViewModel()
    {
        KeyCodes = new List<GenericKey>();
        KeyNames = new List<string>();
    }

    public TriggerViewModel(TriggerViewModel triggerViewModel)
    {
        KeyCodes = triggerViewModel.KeyCodes.ToList();
        KeyNames = triggerViewModel.KeyNames.ToList();
    }
}
