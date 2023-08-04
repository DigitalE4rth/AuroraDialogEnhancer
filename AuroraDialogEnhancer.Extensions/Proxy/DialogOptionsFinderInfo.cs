using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionsFinderInfo
{
    public Rectangle CaptureArea { get; }

    public IDialogOptionsFinder DialogOptionsFinder { get; }

    public DialogOptionsFinderInfo(Rectangle captureArea, IDialogOptionsFinder dialogOptionsFinder)
    {
        CaptureArea = captureArea;
        DialogOptionsFinder = dialogOptionsFinder;
    }
}
