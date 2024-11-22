using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Hooks.Game;

namespace AuroraDialogEnhancer.Backend.Extensions;

public class ExtensionConfigViewModel : INotifyPropertyChanged
{
    public ExtensionConfig Config;

    public ExtensionConfigViewModel(ExtensionConfig config)
    {
        Config = config;
    }

    public string AppId
    {
        get => Config.Id;
        set
        {
            Config.Id = value;
            OnPropertyChanged();
        }
    }

    public string AppLocation
    {
        get => Config.GameLocation;
        set
        {
            Config.GameLocation = value;
            OnPropertyChanged();
        }
    }

    public string LauncherLocation
    {
        get => Config.LauncherLocation;
        set
        {
            Config.LauncherLocation = value;
            OnPropertyChanged();
        }
    }

    public string ScreenshotsLocation
    {
        get => string.IsNullOrEmpty(Config.ScreenshotsLocation) 
            ? Path.Combine(AppConstants.Locations.ExtensionsFolder, Config.Name, AppConstants.Locations.ScreenshotsFolderName)
            : Config.ScreenshotsLocation;
        set
        {
            Config.ScreenshotsLocation = value;
            OnPropertyChanged();
        }
    }

    public EHookLaunchType HookLaunchType
    {
        get => Config.HookLaunchType;
        set
        {
            Config.HookLaunchType = value;
            OnPropertyChanged();
        }
    }

    public string GameProcessName
    {
        get => Config.GameProcessName;
        set
        {
            Config.GameProcessName = value;
            OnPropertyChanged();
        }
    }

    public string LauncherProcessName
    {
        get => Config.LauncherProcessName;
        set
        {
            Config.LauncherProcessName = value;
            OnPropertyChanged();
        }
    }

    public bool IsExitWithTheGame
    {
        get => Config.IsExitWithTheGame;
        set
        {
            Config.IsExitWithTheGame = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
