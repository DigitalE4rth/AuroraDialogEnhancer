using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Frontend.Forms.Main;
using AuroraDialogEnhancer.Frontend.Generics;
using AuroraDialogEnhancer.Frontend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Frontend.Forms;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    #region Initialization
    public void Initialize(bool isForceShow = false, bool isProfileShortcutLaunch = false)
    {
        InitializeSettings();
        InitializeState(isForceShow, isProfileShortcutLaunch);
        InitializeEvents();
        InitializeFrames();
    }

    #region Settings
    private void InitializeSettings()
    {
        InitializeTitle();
        InitializeLocation();
        InitializeSize();
    }

    private void InitializeTitle()
    {
        Title = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyTitleAttribute>().Title;
#if DEBUG
        Title += " | DEVELOPMENT";
#endif
    }

    private void InitializeLocation()
    {
        if (Properties.Settings.Default.UI_MainWindow_Top == 0 && Properties.Settings.Default.UI_MainWindow_Left == 0)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        else
        {
            Top  = Properties.Settings.Default.UI_MainWindow_Top;
            Left = Properties.Settings.Default.UI_MainWindow_Left;
        }
    }

    private void InitializeSize()
    {
        Width  = Properties.Settings.Default.UI_MainWindow_Width;
        Height = Properties.Settings.Default.UI_MainWindow_Height;
    }

    private void InitializeState(bool isForceShow = false, bool isShortcutLaunch = false)
    {
        if (isForceShow)
        {
            WindowState = (WindowState) Properties.Settings.Default.UI_MainWindow_State;
            Topmost = true;
            Show();
            Activate();
            Topmost = false;
            return;
        }

        var startUpState = isShortcutLaunch
            ? (EWindowState) Properties.Settings.Default.UI_MainWindow_State_Shortcut
            : (EWindowState) Properties.Settings.Default.UI_MainWindow_State_Startup;

        if (startUpState != EWindowState.Default)
        {
            if (startUpState == EWindowState.SystemTray) return;

            WindowState = WindowState.Minimized;
            Show();
            return;
        }

        WindowState = (WindowState) Properties.Settings.Default.UI_MainWindow_State;
        Topmost = true;
        Show();
        Activate();
        Topmost = false;
    }
    #endregion

    #region Events
    private void InitializeEvents()
    {
        Closing         += MainWindow_Closing;
        SizeChanged     += MainWindow_SizeChanged;
        LocationChanged += MainWindow_LocationChanged;
        StateChanged    += MainWindow_StateChanged;
    }
    #endregion

    #region Frames
    private void InitializeFrames()
    {
        MainFrame.NavigationService.Navigate(AppServices.ServiceProvider.GetRequiredService<MainPage>());
        MainFrame.NavigationService.RemoveBackEntry();
    }

    public void ReloadMainFrame() => InitializeFrames();
    #endregion
    #endregion


    #region Handlers
    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Properties.Settings.Default.UI_MainWindow_Width  = e.NewSize.Width;
        Properties.Settings.Default.UI_MainWindow_Height = e.NewSize.Height;
        Properties.Settings.Default.Save();
    }

    private void MainWindow_LocationChanged(object sender, EventArgs e)
    {
        Properties.Settings.Default.UI_MainWindow_Top  = Top;
        Properties.Settings.Default.UI_MainWindow_Left = Left;
        Properties.Settings.Default.Save();
    }

    private void MainWindow_StateChanged(object sender, EventArgs e)
    {
        if (WindowState != WindowState.Minimized)
        {
            Properties.Settings.Default.UI_MainWindow_State = (int) WindowState;
            Properties.Settings.Default.Save();
        }

        if (WindowState == WindowState.Minimized)
        {
            Properties.Settings.Default.UI_MainWindow_Top  = RestoreBounds.Top;
            Properties.Settings.Default.UI_MainWindow_Left = RestoreBounds.Left;
            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.App_IsMinimizeToTaskBar) return;

            Hide();
            if (!Properties.Settings.Default.App_IsUnloadUi) return;

            AppServices.ServiceProvider.GetRequiredService<UiService>().DisposeMainWindow();
            return;
        }

        Top  = Properties.Settings.Default.UI_MainWindow_Top;
        Left = Properties.Settings.Default.UI_MainWindow_Left;
        Activate();
    }

    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (Properties.Settings.Default.App_IsCloseMinimizes)
        {
            if (Properties.Settings.Default.App_IsMinimizeToTaskBar)
            {
                e.Cancel    = true;
                WindowState = WindowState.Minimized;
                return;
            }

            WindowState = WindowState.Minimized;
            return;
        }

        if (WindowState == WindowState.Minimized) return;

        Application.Current.Dispatcher.Invoke(Application.Current.Shutdown, DispatcherPriority.Send);
    }

    protected override void OnClosed(EventArgs e)
    {
        StateChanged    -= MainWindow_StateChanged;
        Closing         -= MainWindow_Closing;
        SizeChanged     -= MainWindow_SizeChanged;
        LocationChanged -= MainWindow_LocationChanged;
        base.OnClosed(e);
    }
    #endregion
}
