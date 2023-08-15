using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionFinderInfo
{
    public Rectangle SpeakerNameArea { get; }

    public Rectangle DialogOptionsArea { get; }

    public IDialogOptionFinder DialogOptionFinder { get; }
    
    public DialogOptionFinderInfo(Rectangle speakerNameArea, Rectangle dialogOptionsArea, IDialogOptionFinder dialogOptionFinder)
    {
        SpeakerNameArea = speakerNameArea;
        DialogOptionsArea = dialogOptionsArea;
        DialogOptionFinder = dialogOptionFinder;
    }
}
