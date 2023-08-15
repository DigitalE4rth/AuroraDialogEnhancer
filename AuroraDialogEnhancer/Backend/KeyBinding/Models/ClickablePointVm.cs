namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

public class ClickablePointVm
{
    public string Id          { get; }
                              
    public string Name        { get; }

    public string Description { get; }

    public string PathIcon    { get; }

    public ActionViewModel ActionViewModel { get; }

    public ClickablePointVm(string id, string name, string description, string pathIcon, ActionViewModel actionViewModel)
    {
        Id              = id;
        Name            = name;
        Description     = description;
        PathIcon        = pathIcon;
        ActionViewModel = actionViewModel;
    }
}
