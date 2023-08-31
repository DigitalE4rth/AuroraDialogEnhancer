using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDialogEnhancer.AppConfig.Localization;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Frontend.Forms.Utils;
using AuroraDialogEnhancer.Frontend.Generics;
using AuroraDialogEnhancer.Frontend.Services;
using IWshRuntimeLibrary;
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

#if DEBUG
        Toggle_ExpertMode.Visibility = Visibility.Visible;
#endif
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
        var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), $"{Global.AssemblyInfo.Name}.lnk");
        var isChecked = (bool) ((CardToggleButton) sender).IsChecked!;

        if (isChecked)
        {
            IWshShortcut shortcut = new WshShell().CreateShortcut(shortcutPath);
            shortcut.TargetPath = Global.Locations.AssemblyExe;
            shortcut.WorkingDirectory = Global.Locations.AssemblyFolder;
            shortcut.IconLocation = Global.Locations.AssemblyExe;

            try
            {
                shortcut.Save();
                Properties.Settings.Default.App_IsStartup = isChecked;
                Properties.Settings.Default.Save();
            }
            catch (Exception exception)
            {
                isChecked = System.IO.File.Exists(shortcutPath);

                ((CardToggleButton) sender).IsChecked = isChecked;
                Properties.Settings.Default.App_IsStartup = isChecked;
                Properties.Settings.Default.Save();

                new InfoDialogBuilder()
                    .SetWindowTitle(Properties.Localization.Resources.Settings_Startup_Exception_Save)
                    .SetMessage(exception.Message + Environment.NewLine + exception.InnerException?.Message)
                    .SetTypeError()
                    .ShowDialog();
            }

            return;
        }

        try
        {
            System.IO.File.Delete(shortcutPath);
            Properties.Settings.Default.App_IsStartup = isChecked;
            Properties.Settings.Default.Save();
        }
        catch (Exception exception)
        {
            isChecked = System.IO.File.Exists(shortcutPath);

            ((CardToggleButton) sender).IsChecked = isChecked;
            Properties.Settings.Default.App_IsStartup = isChecked;
            Properties.Settings.Default.Save();

            new InfoDialogBuilder()
                .SetWindowTitle(Properties.Localization.Resources.Settings_Startup_Exception_Delete)
                .SetMessage(exception.Message + Environment.NewLine + exception.InnerException?.Message)
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
        Properties.Settings.Default.App_UnloadUi = (bool) ((CardToggleButton) sender).IsChecked!;
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
