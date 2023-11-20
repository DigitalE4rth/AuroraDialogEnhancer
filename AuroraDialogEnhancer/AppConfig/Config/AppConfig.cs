using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AuroraDialogEnhancer.AppConfig.Localization;
using AuroraDialogEnhancer.AppConfig.NotifyIcon;
using AuroraDialogEnhancer.AppConfig.Theme;
using AuroraDialogEnhancer.AppConfig.Updater;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Frontend.Generics;
using AuroraDialogEnhancer.Frontend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.AppConfig.Config;

public class AppConfig : IDisposable
{
    private readonly AutoUpdaterService    _autoUpdaterService;
    private readonly CoreService           _coreService;
    private readonly CultureService        _cultureService;
    private readonly NotifyIconService     _notifyIconService;
    private readonly ExtensionsLoader      _extensionsLoader;
    private readonly SingleInstanceService _singleInstanceService;
    private readonly UiService             _uiService;

    public AppConfig(AutoUpdaterService    autoUpdaterService,
                     CoreService           coreService,
                     CultureService        cultureService,
                     NotifyIconService     notifyIconService,
                     ExtensionsLoader      extensionsLoader,
                     UiService             uiService,
                     SingleInstanceService singleInstanceService)
    {
        _autoUpdaterService    = autoUpdaterService;
        _coreService           = coreService;
        _cultureService        = cultureService;
        _notifyIconService     = notifyIconService;
        _extensionsLoader      = extensionsLoader;
        _uiService             = uiService;
        _singleInstanceService = singleInstanceService;
    }

    public bool Initialize(StartupEventArgs startupEventArgs)
    {
        if (!IsApplicationInstanceUnique(startupEventArgs)) return false;

        var isUpdated = _autoUpdaterService.UpdateSettings();
        _cultureService.Initialize();
        _extensionsLoader.Initialize();
        if (isUpdated) _autoUpdaterService.PatchKeyBindings();
        InitializeColorTheme();
        _notifyIconService.Initialize();
        _uiService.SetInitialPage();

        var startupProfileId = GetArgumentProfileId(startupEventArgs);
        ShowMainWindow(!string.IsNullOrEmpty(startupProfileId), isUpdated);
        StartAutoDetectionByLaunchArgument(startupProfileId);
        _autoUpdaterService.CheckForUpdateAuto();
        return true;
    }

    private void InitializeColorTheme()
    {
        DependencyInjection.AppServices.ServiceProvider.GetRequiredService<ColorThemeService>().ApplySystemThemeRelatedToWindowsThemeIfNecessary();
    }

    private bool IsApplicationInstanceUnique(StartupEventArgs startupEventArgs)
    {
        if (_singleInstanceService.StartIpcServer())
        {
            _singleInstanceService.OnNewInstance += SingleInstanceService_OnNewInstance;
            return true;
        }

        _singleInstanceService.NotifyFirstInstance(startupEventArgs.Args);
        Application.Current.Dispatcher.Invoke(Application.Current.Shutdown, DispatcherPriority.Send);
        return false;
    }

    private void SingleInstanceService_OnNewInstance(object sender, SingleInstanceEventArgs message)
    {
        var startupProfileId = string.Empty;

        for (var i = 0; i < message.Arguments.Length; i++)
        {
            if (message.Arguments[i].Equals(Properties.DefaultSettings.Default.App_StartupArgument_Profile, StringComparison.Ordinal) &&
                message.Arguments.Length - 1 >= i + 1)
            {
                startupProfileId = message.Arguments[i + 1];
            }
        }

        if (!string.IsNullOrEmpty(startupProfileId))
        {
            Task.Run(() => _coreService.RestartAutoDetection(startupProfileId)).ConfigureAwait(false);
            ShowMainWindow(true);
            return;
        }

        Application.Current.Dispatcher.Invoke(() => _uiService.ShowMainWindow(true));
    }

    private void ShowMainWindow(bool isProfileStartup = false, bool isForceShow = false)
    {
        if (isForceShow && Properties.Settings.Default.App_IsShowMainWindowOnUpdate)
        {
            _uiService.ShowMainWindow(true);
            return;
        }

        if (isProfileStartup)
        {
            if (_uiService.IsMainWindowShown() || (EWindowState) Properties.Settings.Default.UI_MainWindow_State_Shortcut == EWindowState.SystemTray) return;
            Application.Current.Dispatcher.Invoke(() => _uiService.ShowMainWindow(false, true));
            return;
        }

        if ((EWindowState) Properties.Settings.Default.UI_MainWindow_State_Startup == EWindowState.SystemTray) return;
        _uiService.ShowMainWindow();
    }

    private string? GetArgumentProfileId(StartupEventArgs startupEventArgs)
    {
        if (!startupEventArgs.Args.Any()) return null;

        for (var i = 0; i < startupEventArgs.Args.Length; i++)
        {
            if (!startupEventArgs.Args[i].Equals(Properties.DefaultSettings.Default.App_StartupArgument_Profile, StringComparison.Ordinal)) continue;
            if (startupEventArgs.Args.Length - 1 < i + 1) return null;
            return startupEventArgs.Args[i + 1];
        }

        return null;
    }

    private void StartAutoDetectionByLaunchArgument(string? profileId)
    {
        if (string.IsNullOrEmpty(profileId)) return;
        Task.Run(() => _coreService.RestartAutoDetection(profileId!)).ConfigureAwait(false);
    }

    public void Restart()
    {
        _singleInstanceService.Dispose();
        Process.Start(Statics.Global.Locations.AssemblyExe);
        Application.Current.Dispatcher.Invoke(Application.Current.Shutdown, DispatcherPriority.Send);
    }

    public void Dispose()
    {
        _singleInstanceService.OnNewInstance -= SingleInstanceService_OnNewInstance;
        _singleInstanceService.Dispose();
        _uiService.DisposeMainWindow();
        _coreService.Dispose();
        _notifyIconService.Dispose();
    }
}
