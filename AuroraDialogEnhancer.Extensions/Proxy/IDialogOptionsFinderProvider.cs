using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public interface IDialogOptionsFinderProvider
{
    public DialogOptionsFinderInfo? GetDialogOptionsFinder(Size clientSize);
}
