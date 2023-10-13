using System.Collections.Generic;
using System.Linq;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

public class TriggerViewModelComparer : IEqualityComparer<TriggerViewModel>
{
    public bool Equals(TriggerViewModel x, TriggerViewModel y)
    {
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.KeyCodes.Count == y.KeyCodes.Count && !x.KeyCodes.Except(y.KeyCodes).Any();
    }

    public int GetHashCode(TriggerViewModel obj)
    {
        return obj.KeyCodes.Aggregate(17, (current, virtualKey) => (current * 7) + virtualKey.GetHashCode());
    }
}
