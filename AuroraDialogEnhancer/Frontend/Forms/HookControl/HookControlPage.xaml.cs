﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.External;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Utils;
using AuroraDialogEnhancer.Frontend.Forms.Utils;
using AuroraDialogEnhancer.Frontend.Providers;
using IWshRuntimeLibrary;
using Application = System.Windows.Application;
using CheckBox = System.Windows.Controls.CheckBox;

namespace AuroraDialogEnhancer.Frontend.Forms.HookControl;

public partial class HookControlPage
{
    private readonly BlobToBitmapImageConverter _blobToBitmapImageConverter;
    private readonly CoreService                _coreService;
    private readonly ExtensionConfigService     _extensionConfigService;
    private readonly ExtensionsProvider         _extensionsProvider;
    private readonly ProcessDataProvider        _processDataProvider;

    private readonly DefaultUiElementsProvider  _defaultUiElementsProvider;
    private          HookSettingsDataContext?   _hookSettingsDataContext;

    public HookControlPage(BlobToBitmapImageConverter blobToBitmapImageConverter,
                           CoreService                coreService, 
                           ExtensionConfigService     extensionConfigService,
                           ExtensionsProvider         extensionsProvider,
                           ProcessDataProvider        processDataProvider)
    {
        _blobToBitmapImageConverter = blobToBitmapImageConverter;
        _coreService                = coreService;
        _extensionConfigService     = extensionConfigService;
        _extensionsProvider         = extensionsProvider;
        _processDataProvider        = processDataProvider;
        _defaultUiElementsProvider  = new DefaultUiElementsProvider();

        InitializeComponent();
        InitializeGames();
        InitializeComboBoxHookLaunchType();
        SetHookInfoText();
        InitializeBackground();
        InitializeHookInfoButtonState();
    }

    #region Startup
    private void InitializeGames()
    {
        var extensionConfig = _extensionConfigService.Get(Properties.Settings.Default.App_HookSettings_SelectedGameId);

        _hookSettingsDataContext = new HookSettingsDataContext(new ExtensionConfigViewModel(extensionConfig));
        DataContext = _hookSettingsDataContext;

        Unloaded += SettingsPage_Unloaded;
        _processDataProvider.OnHookStateChanged += OnHookDataStateChanged;
        GameSelector.OnGameChanged += GameSelector_OnGameChanged;
    }

    private void InitializeHookInfoButtonState()
    {
        Application.Current.Dispatcher.Invoke(() => { CardButtonHookInfo.IsEnabled = !_coreService.IsProcessing; });
        _coreService.OnProcessing += CoreServiceOnProcessing;
    }

    private void CoreServiceOnProcessing(object sender, bool e)
    {
        Application.Current.Dispatcher.Invoke(() => { CardButtonHookInfo.IsEnabled = !e; });
    }

    private void ComboBoxSettings_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedIndex = ((WhyOrchid.Controls.ComboBox)sender).SelectedIndex;
        if (selectedIndex == -1) return;
        ((WhyOrchid.Controls.ComboBox)sender).SelectedIndex = -1;

        switch (selectedIndex)
        {
            case 0:
                LocatePaths();
                break;
            case 1:
                ResetConfig();
                break;
        }
    }

    private void LocatePaths()
    {
        var config = _extensionConfigService.UpdateLocations(Properties.Settings.Default.App_HookSettings_SelectedGameId);
        if (config is null) return;

        _hookSettingsDataContext!.ExtensionConfig.AppLocation = config.GameLocation;
        _hookSettingsDataContext.ExtensionConfig.LauncherLocation = config.LauncherLocation;
        _hookSettingsDataContext.ExtensionConfig.ScreenshotsLocation = config.ScreenshotsLocation;
    }

    private void ResetConfig()
    {
        _extensionConfigService.SaveDefault(Properties.Settings.Default.App_HookSettings_SelectedGameId);

        SettingsPage_Unloaded(this, new RoutedEventArgs());

        InitializeGames();
        InitializeComboBoxHookLaunchType();
    }

    private void GameSelector_OnGameChanged(object sender, EventArgs e)
    {
        ComboBoxHookLaunchType.SelectionChanged -= ComboBox_HookLaunchType_OnSelectionChanged;

        _hookSettingsDataContext!.ExtensionConfig = _extensionConfigService.GerViewModel(Properties.Settings.Default.App_HookSettings_SelectedGameId);
        ComboBoxHookLaunchType.SelectedItem = ComboBoxHookLaunchType.Items.OfType<ComboBoxItem>().First(item => (EHookLaunchType) item.Tag == _hookSettingsDataContext!.ExtensionConfig.HookLaunchType);
        SetHookInfoText();
        InitializeBackground();

        ComboBoxHookLaunchType.SelectionChanged += ComboBox_HookLaunchType_OnSelectionChanged;
    }

    private void InitializeBackground()
    {
        var cover = _extensionsProvider.ExtensionsDictionary[Properties.Settings.Default.App_HookSettings_SelectedGameId].GetCover();
        if (cover.Width == 0 || cover.Height == 0) return;

        var coverImage = _blobToBitmapImageConverter.Convert(cover);

        CanvasContainer.Width = coverImage.Width;
        HookControlBackgroundImage.Margin = new Thickness(0, -coverImage.Height/4, 0, 0);
        HookControlBackgroundImage.Source = coverImage;
    }

    private void InitializeComboBoxHookLaunchType()
    {
        ComboBoxHookLaunchType.SelectedItem = ComboBoxHookLaunchType.Items.OfType<ComboBoxItem>().First(item => (EHookLaunchType) item.Tag == _hookSettingsDataContext!.ExtensionConfig.HookLaunchType);
        ComboBoxHookLaunchType.SelectionChanged += ComboBox_HookLaunchType_OnSelectionChanged;
    }
    #endregion


    #region Hook settings
    private void OnHookDataStateChanged(object sender, EventArgs e) => Application.Current.Dispatcher.Invoke(SetHookInfoText);

    private void SetHookInfoText()
    {
        PathIconHookInfoRight.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Small;
        if (_processDataProvider.Id != Properties.Settings.Default.App_HookSettings_SelectedGameId)
        {
            PathIconHookInfoRight.Data = (PathGeometry) Application.Current.Resources["I.S.PlayArrow"];
            CardButtonHookInfo.Content = _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_None);
            return;
        }

        string iconName;
        List<UIElement>? hookContent;

        switch (_processDataProvider.HookState)
        {
            case EHookState.Canceled:
                iconName = "I.S.Spinner";
                hookContent = new List<UIElement> { _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_Canceled) };
                break;
            case EHookState.None:
                iconName = "I.S.PlayArrow";
                hookContent = new List<UIElement> { _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_None) };
                break;
            case EHookState.Error:
                iconName = "I.S.Exclamation";
                PathIconHookInfoRight.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Medium;
                hookContent = new List<UIElement>
                {
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_Error), 
                    _defaultUiElementsProvider.GetDivider(),
                    _defaultUiElementsProvider.GetTextBlock(_processDataProvider.Message)
                };
                break;
            case EHookState.Search:
                iconName = "I.S.Stop";
                hookContent = new List<UIElement>
                {
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_Search), 
                    _defaultUiElementsProvider.GetDivider(),
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_Search_PressToStop)
                };
                break;
            case EHookState.Switch:
                // ToDo: add switch description
                iconName = "I.R.SwitchAccess";
                hookContent = new List<UIElement>
                {
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_Error), 
                    _defaultUiElementsProvider.GetDivider(),
                    _defaultUiElementsProvider.GetTextBlock(_processDataProvider.Message)
                };
                break;
            case EHookState.Paused:
                iconName = "I.S.PlayArrow";
                hookContent = new List<UIElement>
                {
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_Paused),
                    _defaultUiElementsProvider.GetDivider(),
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.WindowInfo_ClientSize + ": " + _processDataProvider.Data!.GameWindowInfo!.GetClientSize())
                };  
                break;
            case EHookState.Hooked:
                iconName = "I.S.Stop";
                hookContent = new List<UIElement>
                {
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_Detected),
                    _defaultUiElementsProvider.GetDivider(),
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.WindowInfo_ClientSize + ": " + _processDataProvider.Data!.GameWindowInfo!.GetClientSize())
                };
                break;
            case EHookState.Warning:
                iconName = "I.S.RotateRight";
                hookContent = new List<UIElement>
                {
                    _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_Warning),
                    _defaultUiElementsProvider.GetDivider(),
                    _defaultUiElementsProvider.GetTextBlock(_processDataProvider.Message)
                };
                break;
            default:
                PathIconHookInfoRight.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Medium;
                iconName = "I.S.Exclamation";
                hookContent = new List<UIElement> { _defaultUiElementsProvider.GetTextBlock(Properties.Localization.Resources.HookSettings_State_AppException) };
                break;
        }

        PathIconHookInfoRight.Data = (PathGeometry) Application.Current.Resources[iconName];
        
        var gridContentHolder = new Grid();
        for (var i = 0; i < hookContent.Count; i++)
        {
            gridContentHolder.ColumnDefinitions.Add(new ColumnDefinition{ Width = GridLength.Auto });
            gridContentHolder.Children.Add(hookContent[i]);
            Grid.SetColumn(hookContent[i], i);
        }

        gridContentHolder.ColumnDefinitions.Last().Width = new GridLength(1, GridUnitType.Star);

        CardButtonHookInfo.Content = gridContentHolder;
    }

    private void Button_StartHook_OnClick(object sender, RoutedEventArgs e)
    {
        _coreService.Run(_hookSettingsDataContext!.ExtensionConfig.AppId);
    }

    private void Button_GameLocation_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            CheckFileExists  = true,
            CheckPathExists  = true,
            RestoreDirectory = true,
            DefaultExt       = "exe",
            Filter           = $"Exe {Properties.Localization.Resources.FileDialog_File} (*.exe)|*.exe|{Properties.Localization.Resources.FileDialog_AllFiles} (*.*)|*.*"
        };
        
        if (dialog.ShowDialog() != DialogResult.OK) return;

        _hookSettingsDataContext!.ExtensionConfig.AppLocation = dialog.FileName;
        _extensionConfigService.SaveAndRestartHookIfNecessary(_hookSettingsDataContext!.ExtensionConfig.Config);
    }

    private void Button_LauncherLocation_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            CheckFileExists  = true,
            CheckPathExists  = true,
            RestoreDirectory = true,
            DefaultExt       = "exe",
            Filter           = $"Exe {Properties.Localization.Resources.FileDialog_File} (*.exe)|*.exe|{Properties.Localization.Resources.FileDialog_AllFiles} (*.*)|*.*"
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;

        _hookSettingsDataContext!.ExtensionConfig.LauncherLocation = dialog.FileName;
        _extensionConfigService.SaveAndRestartHookIfNecessary(_hookSettingsDataContext!.ExtensionConfig.Config);
    }

    private void Button_SetGameProcessName_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputResetDialog(new TextInputResetDialogDataContext
        {
            TextBoxTitle = Properties.Localization.Resources.HookSettings_ProcessName_Game,
            TextContent  = _hookSettingsDataContext!.ExtensionConfig.GameProcessName,
            ResetContent = _extensionsProvider.ExtensionsDictionary[Properties.Settings.Default.App_HookSettings_SelectedGameId].GetConfig().GameProcessName
        })
        {
            Owner = Application.Current.MainWindow,
        };

        if (dialog.ShowDialog() != true) return;

        _hookSettingsDataContext.ExtensionConfig.GameProcessName = dialog.Result!;
        _extensionConfigService.SaveAndRestartHookIfNecessary(_hookSettingsDataContext!.ExtensionConfig.Config);
    }

    private void Button_SetLauncherProcessName_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputResetDialog(new TextInputResetDialogDataContext
        {
            TextBoxTitle = Properties.Localization.Resources.HookSettings_ProcessName_Launcher,
            TextContent  = _hookSettingsDataContext!.ExtensionConfig.LauncherProcessName,
            ResetContent = _extensionsProvider.ExtensionsDictionary[Properties.Settings.Default.App_HookSettings_SelectedGameId].GetConfig().LauncherProcessName
        })
        {
            Owner = Application.Current.MainWindow
        };

        if (dialog.ShowDialog() != true) return;

        _hookSettingsDataContext.ExtensionConfig.LauncherProcessName = dialog.Result!;
        _extensionConfigService.SaveAndRestartHookIfNecessary(_hookSettingsDataContext!.ExtensionConfig.Config);
    }
    #endregion


    #region Additional settings
    private void CardButton_OpenScreenShotsFolder(object sender, RoutedEventArgs e)
    {
        new FolderProcessStartService().Open(_hookSettingsDataContext!.ExtensionConfig.ScreenshotsLocation);
    }

    private void Button_ScreenShotsLocation_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new FolderPicker
        {
            InputPath = new OpenFileDialog().InitialDirectory
        };

        if (dialog.ShowDialog() != true) return;

        _hookSettingsDataContext!.ExtensionConfig.ScreenshotsLocation = dialog.ResultPath!;
        _extensionConfigService.SaveAndRestartHookIfNecessary(_hookSettingsDataContext.ExtensionConfig.Config);
        _extensionConfigService.SetScreenshotsFolderForActiveGameIfNecessary(_hookSettingsDataContext.ExtensionConfig.Config);
    }

    private void ComboBox_HookLaunchType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _hookSettingsDataContext!.ExtensionConfig.Config.HookLaunchType = (EHookLaunchType) ((ComboBoxItem)e.AddedItems[0]).Tag;
        _extensionConfigService.SaveAndRestartHookIfNecessary(_hookSettingsDataContext.ExtensionConfig.Config);
    }

    private void CardButton_CreateShortcut_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            CheckPathExists  = true,
            RestoreDirectory = true,
            DefaultExt       = "lnk",
            Title            = Properties.Localization.Resources.FileDialog_Shortcut_Title,
            FileName         = $"{_hookSettingsDataContext!.ExtensionConfig.Config.Name} (ADE)",
            Filter           = $"lnk {Properties.Localization.Resources.FileDialog_Files} (*.lnk)|*.lnk|{Properties.Localization.Resources.FileDialog_AllFiles} (*.*)|*.*"
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;
        
        IWshShortcut shortcut     = new WshShell().CreateShortcut(dialog.FileName);
        shortcut.Arguments        = $"{Properties.DefaultSettings.Default.App_StartupArgument_Profile} {_hookSettingsDataContext!.ExtensionConfig.Config.Id}";
        shortcut.Description      = $"ADE Profile: {_hookSettingsDataContext.ExtensionConfig.Config.Name}";
        shortcut.TargetPath       = AppConstants.Locations.AssemblyExe;
        shortcut.WorkingDirectory = AppConstants.Locations.AssemblyFolder;
        shortcut.IconLocation     = AppConstants.Locations.AssemblyExe;
        shortcut.Save();
    }

    private void CheckBox_ExitWithTheGame_OnClick(object sender, RoutedEventArgs e)
    {
        _hookSettingsDataContext!.ExtensionConfig.Config.IsExitWithTheGame = (bool) ((CheckBox) e.Source).IsChecked!;
        _extensionConfigService.SaveAndRestartHookIfNecessary(_hookSettingsDataContext.ExtensionConfig.Config);
    }
    #endregion

    #region Utils
    private void SetErrorMessage()
    {
        _processDataProvider.SetStateAndNotify(EHookState.Warning, Properties.Localization.Resources.HookSettings_Warning_UnappliedChanges);
    }
    #endregion

    #region Cleaunup
    private void SettingsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= SettingsPage_Unloaded;
        GameSelector.OnGameChanged -= GameSelector_OnGameChanged;
        ComboBoxHookLaunchType.SelectionChanged -= ComboBox_HookLaunchType_OnSelectionChanged;
        _processDataProvider.OnHookStateChanged -= OnHookDataStateChanged;
        _coreService.OnProcessing -= CoreServiceOnProcessing;
    }
    #endregion
}
