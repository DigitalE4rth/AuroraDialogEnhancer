using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Frontend.Forms;
using AuroraDialogEnhancer.Frontend.Forms.About;
using AuroraDialogEnhancer.Frontend.Forms.Appearance;
using AuroraDialogEnhancer.Frontend.Forms.Debug;
using AuroraDialogEnhancer.Frontend.Forms.HookControl;
using AuroraDialogEnhancer.Frontend.Forms.KeyBinding;
using AuroraDialogEnhancer.Frontend.Forms.Menu;
using AuroraDialogEnhancer.Frontend.Forms.PresetsEditor;
using AuroraDialogEnhancer.Frontend.Forms.Settings;
using AuroraDialogEnhancer.Frontend.Forms.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Frontend.Services;

public class UiService
{
    private MainWindow?     _mainWindow;
    private readonly object _mainWindowLock;
    private readonly Dictionary<EPageType, Type> _pageTypesDictionary;
    private Frame? _mainFrame;

    public bool IsMainWindowExist => _mainWindow is not null;

    public EPageType CurrentPage { get; private set; } 

    public UiService()
    {
        CurrentPage     = EPageType.HookSettings;
        _mainWindowLock = new object();

        _pageTypesDictionary = new Dictionary<EPageType, Type>
        {
            { EPageType.Appearance,     typeof(AppearancePage)        },
            { EPageType.HookSettings,   typeof(HookControlPage)       },
            { EPageType.KeyBinding,     typeof(KeyBindingPage)        },
            { EPageType.Settings,       typeof(SettingsPage)          },
            { EPageType.About,          typeof(AboutPage)             },
            { EPageType.PresetsEditor,  typeof(PresetsEditorPage)     },
            { EPageType.Debug,          typeof(DebugPage)             },
            { EPageType.MissingPresets, typeof(MissingExtensionsPage) }
        };
    }

    public void InitializeMainFrame(Frame frame) => _mainFrame = frame;

    public void SetNewRoute(EPageType pageType, Type route) => _pageTypesDictionary[pageType] = route;

    public void Navigate(EPageType pageType)
    {
        CurrentPage = pageType;
        _mainFrame!.NavigationService.Navigate(AppServices.ServiceProvider.GetRequiredService(_pageTypesDictionary[pageType]));
        _mainFrame.NavigationService.RemoveBackEntry();
    }

    public MainWindow GetMainWindow()
    {
        lock (_mainWindowLock)
        {
            if (_mainWindow is not null) return _mainWindow;

            _mainWindow = AppServices.ServiceProvider.GetRequiredService<MainWindow>();
            _mainWindow.Initialize();
            return _mainWindow;
        }
    }

    public void ShowMainWindow(bool isProfileShortcutLaunch = false)
    {
        lock (_mainWindowLock)
        {
            if (_mainWindow is null)
            {
                _mainWindow = AppServices.ServiceProvider.GetRequiredService<MainWindow>();
                _mainWindow!.Initialize(isProfileShortcutLaunch);
                return;
            }

            _mainWindow!.Show();
            if (_mainWindow.WindowState == WindowState.Minimized)
            {
                _mainWindow.WindowState = WindowState.Normal;
            }

            _mainWindow.Activate();
            _mainWindow.Focus();
        }
    }

    public void SetInitialPage()
    {
        if (Properties.Settings.Default.UI_InitialPage == -1) return;

        CurrentPage = (EPageType) Properties.Settings.Default.UI_InitialPage;
        Properties.Settings.Default.UI_InitialPage = -1;
        Properties.Settings.Default.Save();
    }

    public void DisposeMainWindow()
    {
        lock (_mainWindowLock)
        {
            _mainWindow?.Close();
            _mainWindow = null;
        }
    }

    public void ReloadUi()
    {
        lock (_mainWindowLock)
        {
            var mainWindow = GetMainWindow();
            mainWindow.ReloadMainFrame();
        }
    }
}
