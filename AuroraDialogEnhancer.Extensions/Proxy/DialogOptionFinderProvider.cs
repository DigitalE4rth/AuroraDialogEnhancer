namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionFinderProvider
{
    public IDialogOptionFinder DialogOptionsFinder { get; }

    public DialogOptionFinderData Data { get; }

    public DialogOptionFinderProvider(IDialogOptionFinder dialogOptionsFinder, DialogOptionFinderData data)
    {
        DialogOptionsFinder = dialogOptionsFinder;
        Data = data;
    }
}
