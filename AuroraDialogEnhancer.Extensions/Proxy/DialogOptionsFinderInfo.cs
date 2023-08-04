using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionsFinderInfo
{
    public Rectangle SpeakerNameArea { get; }

    public Rectangle DialogOptionsArea { get; }

    public IDialogOptionsFinder DialogOptionsFinder { get; }
    
    public DialogOptionsFinderInfo(Rectangle speakerNameArea, Rectangle dialogOptionsArea, IDialogOptionsFinder dialogOptionsFinder)
    {
        SpeakerNameArea = speakerNameArea;
        DialogOptionsArea = dialogOptionsArea;
        DialogOptionsFinder = dialogOptionsFinder;
    }
}
