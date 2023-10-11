using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

namespace AuroraDialogEnhancer.Frontend.Forms.KeyBinding;

public class AutoSkipDataContext : INotifyPropertyChanged
{
    public AutoSkipConfigViewModel AutoSkipConfigViewModel { get; }

    public ActionViewModel ActivationKeys
    {
        get => AutoSkipConfigViewModel.ActivationKeys;
        set => AutoSkipConfigViewModel.ActivationKeys = value;
    }

    public EAutoSkipType AutoSkipType
    {
        get => AutoSkipConfigViewModel.AutoSkipType;
        set => AutoSkipConfigViewModel.AutoSkipType = value;
    }

    public TriggerViewModel SkipKeys
    {
        get => AutoSkipConfigViewModel.SkipKeys;
        set => AutoSkipConfigViewModel.SkipKeys = value;
    }

    public int Delay
    {
        get => AutoSkipConfigViewModel.Delay;
        set
        {
            AutoSkipConfigViewModel.Delay = value;
            OnPropertyChanged();
        }
    }

    public bool IsDoubleClickDelay
    {
        get => AutoSkipConfigViewModel.IsDoubleClickDelay;
        set => AutoSkipConfigViewModel.IsDoubleClickDelay = value;
    }

    public int DoubleClickDelay
    {
        get => AutoSkipConfigViewModel.DoubleClickDelay;
        set
        {
            AutoSkipConfigViewModel.DoubleClickDelay = value;
            OnPropertyChanged();
        }
    }

    public AutoSkipDataContext(AutoSkipConfigViewModel autoSkipConfigViewModel)
    {
        AutoSkipConfigViewModel = autoSkipConfigViewModel;
    }

    public AutoSkipDataContext()
    {
        AutoSkipConfigViewModel = new AutoSkipConfigViewModel();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
