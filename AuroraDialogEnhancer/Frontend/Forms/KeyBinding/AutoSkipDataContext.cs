using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;

namespace AuroraDialogEnhancer.Frontend.Forms.KeyBinding;

public class AutoSkipDataContext : INotifyPropertyChanged
{
    public AutoSkipConfigViewModel ViewModel { get; }

    public ActionViewModel ActivationKeys
    {
        get => ViewModel.ActivationKeys;
        set => ViewModel.ActivationKeys = value;
    }

    public ESkipMode SkipMode
    {
        get => ViewModel.SkipMode;
        set => ViewModel.SkipMode = value;
    }

    public ESkipStartCondition StartCondition
    {
        get => ViewModel.StartCondition;
        set => ViewModel.StartCondition = value;
    }

    public TriggerViewModel SkipKeys
    {
        get => ViewModel.SkipKeys;
        set => ViewModel.SkipKeys = value;
    }

    public int Delay
    {
        get => ViewModel.Delay;
        set
        {
            ViewModel.Delay = value;
            OnPropertyChanged();
        }
    }

    public bool IsDoubleClickDelay
    {
        get => ViewModel.IsDoubleClickDelay;
        set => ViewModel.IsDoubleClickDelay = value;
    }

    public int DoubleClickDelay
    {
        get => ViewModel.DoubleClickDelay;
        set
        {
            ViewModel.DoubleClickDelay = value;
            OnPropertyChanged();
        }
    }

    public AutoSkipDataContext(AutoSkipConfigViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    public AutoSkipDataContext()
    {
        ViewModel = new AutoSkipConfigViewModel();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
