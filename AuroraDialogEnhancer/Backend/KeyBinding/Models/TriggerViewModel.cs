using System.Collections.Generic;
using System.Linq;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

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

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return ((TriggerViewModel)obj).KeyCodes.Count == KeyCodes.Count && !KeyCodes.Except(((TriggerViewModel)obj).KeyCodes).Any();
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return KeyCodes.Aggregate(17, (current, virtualKey) => (current * 7) + virtualKey.GetHashCode());
    }
}
