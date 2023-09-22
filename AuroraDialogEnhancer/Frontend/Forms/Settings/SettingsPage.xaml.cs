using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using AuroraDialogEnhancer.AppConfig.Localization;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Frontend.Forms.Utils;
using AuroraDialogEnhancer.Frontend.Generics;
using AuroraDialogEnhancer.Frontend.Services;
using Microsoft.Win32;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Forms.Settings;

public partial class SettingsPage
{
    private readonly CultureService _cultureService;
    private readonly UiService      _uiService;

    public SettingsPage(CultureService cultureService, 
                        UiService      uiService)
    {
        _cultureService = cultureService;
        _uiService      = uiService;

        InitializeComponent();

        Unloaded += SettingsPage_Unloaded;
        InitializeComboBoxLanguage();
        InitializeComboBoxMainWindowStartupState();
        InitializeComboBoxMainWindowShortcutState();
        InitializeDescriptionFallback();
    }

    #region Initialization
    private void InitializeComboBoxLanguage()
    {
        var availableLanguages = _cultureService.CultureProvider.GetAvailableCultures();
        ComboBoxLanguages.ItemsSource = availableLanguages;
        ComboBoxLanguages.SelectedIndex = availableLanguages.IndexOf(availableLanguages.First(ci => ci.IetfLanguageTag.Equals(Properties.Settings.Default.App_CurrentCulture, StringComparison.Ordinal)));
        ComboBoxLanguages.SelectionChanged += ComboBoxLanguages_OnSelectionChanged;
    }

    private void InitializeComboBoxMainWindowStartupState()
    {
        ComboBoxStartupWindowState.SelectedItem = ComboBoxStartupWindowState.Items.OfType<ComboBoxItem>().First(item => (EWindowState) item.Tag == (EWindowState) Properties.Settings.Default.UI_MainWindow_State_Startup);
        ComboBoxStartupWindowState.SelectionChanged += ComboBoxStartupWindowState_OnSelectionChanged;
    }

    private void InitializeComboBoxMainWindowShortcutState()
    {
        ComboBoxShortcutWindowState.SelectedItem = ComboBoxShortcutWindowState.Items.OfType<ComboBoxItem>().First(item => (EWindowState) item.Tag == (EWindowState) Properties.Settings.Default.UI_MainWindow_State_Shortcut);
        ComboBoxShortcutWindowState.SelectionChanged += ComboBoxShortcutWindowState_OnSelectionChanged;
    }
    #endregion

    #region General
    private void ComboBoxLanguages_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cultureInfo = (CultureInfo) e.AddedItems[0];
        Properties.Settings.Default.App_CurrentCulture = cultureInfo.IetfLanguageTag;
        Properties.Settings.Default.Save();

        _cultureService.Initialize();
        _uiService.ReloadUi();
    }

    private void ComboBoxStartupWindowState_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Properties.Settings.Default.UI_MainWindow_State_Startup = (int) ((ComboBoxItem) e.AddedItems[0]).Tag;
        Properties.Settings.Default.Save();
    }

    private void ComboBoxShortcutWindowState_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Properties.Settings.Default.UI_MainWindow_State_Shortcut = (int) ((ComboBoxItem) e.AddedItems[0]).Tag;
        Properties.Settings.Default.Save();
    }

    private void CloseMinimizes_OnClick(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.App_IsCloseMinimizes = (bool) ((CardToggleButton) sender).IsChecked!;
        Properties.Settings.Default.Save();
    }

    private void MinimizeToTaskBar_OnClick(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.App_IsMinimizeToTaskBar = (bool) ((CardToggleButton) sender).IsChecked!;
        Properties.Settings.Default.Save();
    }

    private void Button_SetStartup_OnClick(object sender, RoutedEventArgs e)
    {
        var isChecked = (bool) ((CardToggleButton) sender).IsChecked!;

        if (isChecked)
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                key!.SetValue(Assembly.GetExecutingAssembly().GetName().Name, Global.Locations.AssemblyExe);
                Properties.Settings.Default.App_IsStartup = true;
                Properties.Settings.Default.Save();
            }
            catch (Exception exception)
            {
                ((CardToggleButton) sender).IsChecked = false;
                Properties.Settings.Default.App_IsStartup = false;
                Properties.Settings.Default.Save();

                new InfoDialogBuilder()
                    .SetWindowTitle(Properties.Localization.Resources.Settings_Startup_Exception_Save)
                    .SetMessage($"{exception.Message}{Environment.NewLine}{exception.InnerException?.Message}")
                    .SetTypeError()
                    .ShowDialog();
            }

            return;
        }

        try
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            key!.DeleteValue(Assembly.GetExecutingAssembly().GetName().Name);
            Properties.Settings.Default.App_IsStartup = false;
            Properties.Settings.Default.Save();
        }
        catch (Exception exception)
        {
            ((CardToggleButton) sender).IsChecked = false;
            Properties.Settings.Default.App_IsStartup = false;
            Properties.Settings.Default.Save();

            new InfoDialogBuilder()
                .SetWindowTitle(Properties.Localization.Resources.Settings_Startup_Exception_Delete)
                .SetMessage($"{exception.Message}{Environment.NewLine}{exception.InnerException?.Message}")
                .SetTypeError()
                .ShowDialog();
        }
    }

    private void Button_ScreenshotManager_OnClick(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.App_IsScreenshotsManager = (bool) ((CardToggleButton) sender).IsChecked!;
        Properties.Settings.Default.Save();
    }

    private void UnloadUi_OnClick(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.App_IsUnloadUi = (bool) ((CardToggleButton) sender).IsChecked!;
        Properties.Settings.Default.Save();
    }
    #endregion

    #region Utilities
    private void ExpertMode_OnClick(object sender, RoutedEventArgs e)
    {
        var cardButton = (CardToggleButton)e.Source;
        var expertMode = (bool)cardButton.IsChecked!;
        Properties.Settings.Default.App_IsExpertMode = expertMode;
        Properties.Settings.Default.Save();

        _uiService.ReloadUi();
    }

    private void Button_OpenApplicationFolder_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            Arguments = AppContext.BaseDirectory,
            UseShellExecute = true,
            FileName = Global.StringConstants.ExplorerName
        });
    }

    private void Button_OpenApplicationSettingsFolder_OnClick(object sender, RoutedEventArgs e)
    {
        const ConfigurationUserLevel level = ConfigurationUserLevel.PerUserRoamingAndLocal;
        var configuration = ConfigurationManager.OpenExeConfiguration(level);
        var configurationDirectory = Path.GetDirectoryName(configuration.FilePath);

        Process.Start(new ProcessStartInfo
        {
            Arguments = configurationDirectory,
            UseShellExecute = true,
            FileName = Global.StringConstants.ExplorerName
        });
    }
    #endregion

    #region Internet
    private void InitializeDescriptionFallback()
    {
        if (string.IsNullOrEmpty(Properties.Settings.Default.WebClient_UserAgent))
        {
            CardButtonUserAgent.Description = Properties.DefaultSettings.Default.WebClient_UserAgent;
        }

        if (string.IsNullOrEmpty(Properties.Settings.Default.Update_UpdateServerUri))
        {
            CardButtonUpdateServer.Description = Properties.DefaultSettings.Default.Update_UpdateServerUri;
        }
    }

    private void Button_SetUserAgent_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputResetDialog(new TextInputResetDialogDataContext
        {
            TextBoxTitle = Properties.Localization.Resources.Settings_UserAgent,
            TextContent  = string.IsNullOrEmpty(Properties.Settings.Default.WebClient_UserAgent) ? Properties.DefaultSettings.Default.WebClient_UserAgent : Properties.Settings.Default.WebClient_UserAgent,
            ResetContent = Properties.DefaultSettings.Default.WebClient_UserAgent
        })
        {
            Owner = Application.Current.MainWindow,
        };

        if (dialog.ShowDialog() != true) return;

        Properties.Settings.Default.WebClient_UserAgent = dialog.Result!.Equals(Properties.DefaultSettings.Default.WebClient_UserAgent) ? string.Empty : dialog.Result;
        Properties.Settings.Default.Save();

        if (string.IsNullOrEmpty(dialog.Result))
        {
            ((CardButton)sender).Description = Properties.DefaultSettings.Default.WebClient_UserAgent;
            return;
        }

        ((CardButton) sender).Description = dialog.Result!;
    }

    private void Button_SetUpdateServer_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputResetDialog(new TextInputResetDialogDataContext
        {
            TextBoxTitle = Properties.Localization.Resources.Settings_Update_AppServer,
            TextContent  = string.IsNullOrEmpty(Properties.Settings.Default.Update_UpdateServerUri) ? Properties.DefaultSettings.Default.Update_UpdateServerUri : Properties.Settings.Default.Update_UpdateServerUri,
            ResetContent = Properties.DefaultSettings.Default.Update_UpdateServerUri
        })
        {
            Owner = Application.Current.MainWindow,
        };

        if (dialog.ShowDialog() != true) return;

        Properties.Settings.Default.Update_UpdateServerUri = dialog.Result!.Equals(Properties.DefaultSettings.Default.Update_UpdateServerUri) ? string.Empty : dialog.Result;
        Properties.Settings.Default.Save();

        if (string.IsNullOrEmpty(dialog.Result))
        {
            ((CardButton)sender).Description = Properties.DefaultSettings.Default.WebClient_UserAgent;
            return;
        }

        ((CardButton) sender).Description = dialog.Result!;
    }
    #endregion
    
    #region Cleanup
    private void SettingsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= SettingsPage_Unloaded;
        ComboBoxLanguages.SelectionChanged -= ComboBoxLanguages_OnSelectionChanged;
        ComboBoxStartupWindowState.SelectionChanged -= ComboBoxStartupWindowState_OnSelectionChanged;
        ComboBoxShortcutWindowState.SelectionChanged -= ComboBoxShortcutWindowState_OnSelectionChanged;
    }
    #endregion
}
