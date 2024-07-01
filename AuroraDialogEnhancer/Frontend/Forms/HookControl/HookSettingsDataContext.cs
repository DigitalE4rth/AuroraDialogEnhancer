using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Frontend.Forms.HookControl;

internal class HookSettingsDataContext : INotifyPropertyChanged
{
    public ProcessDataProvider ProcessDataProvider { get; set; }

    private ExtensionConfigViewModel _extensionConfig;

    public ExtensionConfigViewModel ExtensionConfig
    {
        get => _extensionConfig;
        set
        {
            _extensionConfig = value;
            OnPropertyChanged();
        }
    }

    public HookSettingsDataContext(ExtensionConfigViewModel extensionConfig)
    {
        ProcessDataProvider = AppServices.ServiceProvider.GetRequiredService<ProcessDataProvider>();
        _extensionConfig = extensionConfig;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
