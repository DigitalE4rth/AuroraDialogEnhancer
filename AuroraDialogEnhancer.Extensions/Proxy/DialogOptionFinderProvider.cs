namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionFinderProvider
{
    public IDialogOptionFinder DialogOptionsFinder { get; }

    public PresetData Data { get; }

    public DialogOptionFinderProvider(IDialogOptionFinder dialogOptionsFinder, PresetData data)
    {
        DialogOptionsFinder = dialogOptionsFinder;
        Data = data;
    }
}
