using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.AppConfig.Theme;
using AuroraDialogEnhancer.Frontend.Controls.ColorPicker;
using AuroraDialogEnhancer.Frontend.Controls.FontPicker;
using AuroraDialogEnhancer.Frontend.Services;
using WhyOrchid.ColorTheme.Models;
using Application = System.Windows.Application;

namespace AuroraDialogEnhancer.Frontend.Forms.Appearance;

public partial class AppearancePage
{
    private bool _isScaleSliderDragged;
    private readonly ColorThemeService _colorThemeService;
    private readonly UiService _uiService;

    public AppearancePage(ColorThemeService colorThemeService, UiService uiService)
    {
        _colorThemeService = colorThemeService;
        _uiService = uiService;

        InitializeComponent();

        InitializeEvents();
        InitializeComboBoxThemes();
        InitializeUiScaleEvents();
        InitializeFontRestoreButtonVisibility();
        InitializeComboBoxCursor();
    }

    #region Initialization
    private void InitializeEvents()
    {
        Unloaded += AppearancePage_OnUnloaded;
    }

    private void InitializeComboBoxThemes()
    {
        var appliedTheme = (EColorTheme) Properties.Settings.Default.UI_ThemeInfo_Type;
        var themeItem    = ComboBoxColorTheme.Items.OfType<ComboBoxItem>().First(item => (EColorTheme) item.Tag == appliedTheme);

        ComboBoxColorTheme.SelectedItem = themeItem;

        ComboBoxItemCustom.Content = string.IsNullOrEmpty(Properties.Settings.Default.UI_ThemeInfo_ThemeName)
            ? Properties.Localization.Resources.Appearance_ColorTheme_Custom
            : Properties.Settings.Default.UI_ThemeInfo_ThemeName;

        ButtonExport.IsEnabled = appliedTheme != EColorTheme.Custom;

        ComboBoxColorTheme.SelectionChanged += ComboBox_ColorTheme_OnSelectionChanged;
    }

    private void InitializeUiScaleEvents()
    {
        SliderScale.Value = Properties.Settings.Default.UI_Scale;
        SliderScale.ValueChanged += SliderScale_OnValueChanged;
    }

    private void InitializeComboBoxCursor()
    {
        var appliedCursorType = WhyOrchid.Properties.Settings.Default.UI_CursorType == Enum.GetName(typeof(EAppCursor), EAppCursor.Arrow) ? EAppCursor.Arrow : EAppCursor.Hand;
        ComboBoxCursor.SelectedItem = ComboBoxCursor.Items.OfType<ComboBoxItem>().First(item => (EAppCursor) item.Tag == appliedCursorType);
        ComboBoxCursor.SelectionChanged += ComboBoxCursorOnSelectionChanged;
    }
    #endregion


    #region Themes
    private void ComboBox_ColorTheme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var colorThemeType = (EColorTheme) ((ComboBoxItem) ComboBoxColorTheme.SelectedItem).Tag;
        if (colorThemeType == EColorTheme.Custom)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.UI_ThemeInfo_Location))
            {
                ImportTheme();
                return;
            }

            ApplyCustomTheme(Properties.Settings.Default.UI_ThemeInfo_Location);
            return;
        }

        ApplyBuiltInTheme(colorThemeType);
    }

    private void ApplyBuiltInTheme(EColorTheme colorThemeType)
    {
        var isThemeApplied = _colorThemeService.Apply(colorThemeType);
        if (!isThemeApplied) return;

        Properties.Settings.Default.UI_ThemeInfo_Type = (int) colorThemeType;
        Properties.Settings.Default.Save();

        _uiService.ReloadUi();
    }

    private void ApplyCustomTheme(string filePath)
    {
        var (isThemeApplied, colorTheme) = _colorThemeService.Apply(filePath);

        if (!isThemeApplied)
        {
            var appliedTheme = (EColorTheme) Properties.Settings.Default.UI_ThemeInfo_Type;
            ComboBoxItemCustom.Content           = Properties.Localization.Resources.Appearance_ColorTheme_Custom;
            ComboBoxColorTheme.SelectionChanged -= ComboBox_ColorTheme_OnSelectionChanged;
            ComboBoxColorTheme.SelectedItem      = ComboBoxColorTheme.Items.OfType<ComboBoxItem>().First(item => (EColorTheme) item.Tag == appliedTheme);
            ComboBoxColorTheme.SelectionChanged += ComboBox_ColorTheme_OnSelectionChanged;
            ButtonExport.IsEnabled = appliedTheme != EColorTheme.Custom;

            Properties.Settings.Default.UI_ThemeInfo_ThemeName = string.Empty;
            Properties.Settings.Default.UI_ThemeInfo_Location  = string.Empty;
            Properties.Settings.Default.Save();
            return;
        }

        ButtonExport.IsEnabled = false;
        ComboBoxItemCustom.Content = colorTheme!.Name;
        Properties.Settings.Default.UI_ThemeInfo_Type      = (int) EColorTheme.Custom;
        Properties.Settings.Default.UI_ThemeInfo_ThemeName = colorTheme.Name;
        Properties.Settings.Default.UI_ThemeInfo_Location  = filePath;
        Properties.Settings.Default.Save();

        _uiService.ReloadUi();
    }

    private void Button_CreateTheme_OnClick(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(Global.Locations.ThemesFolder);

        var dialog = new SaveFileDialog
        {
            InitialDirectory = Global.Locations.ThemesFolder,
            RestoreDirectory = true,
            CheckPathExists  = true,
            FileName   = "ADE_Theme",
            DefaultExt = "xml",
            Filter     = $@"Xml {Properties.Localization.Resources.FileDialog_File} (*.xml)|*.xml"
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;
        _colorThemeService.Serialize(dialog.FileName, new ColorTheme());
    }

    private void Button_ImportTheme_OnClick(object sender, RoutedEventArgs e) => ImportTheme();

    private void ImportTheme()
    {
        Directory.CreateDirectory(Global.Locations.ThemesFolder);

        var dialog = new OpenFileDialog
        {
            InitialDirectory = Global.Locations.ThemesFolder,
            CheckFileExists  = true,
            CheckPathExists  = true,
            RestoreDirectory = true,
            DefaultExt       = "xml",
            Filter           = $@"Xml {Properties.Localization.Resources.FileDialog_File} (*.xml)|*.xml"
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;
        ApplyCustomTheme(dialog.FileName);
    }

    private void Button_ExportTheme_OnClick(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(Global.Locations.ThemesFolder);

        var (themeName, theme) = _colorThemeService.GetAppliedTheme();
        var dialog = new SaveFileDialog
        {
            InitialDirectory = Global.Locations.ThemesFolder,
            CheckPathExists  = true,
            RestoreDirectory = true,
            FileName         = themeName,
            DefaultExt       = "xml",
            Filter           = $@"Xml {Properties.Localization.Resources.FileDialog_File} (*.xml)|*.xml"
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;
        _colorThemeService.Serialize(dialog.FileName, theme);
    }
    #endregion


    private void Button_AccentColor_OnClick(object sender, RoutedEventArgs e)
    {
        Color userAccentColor;
        try
        {
            userAccentColor = (Color) ColorConverter.ConvertFromString(Properties.Settings.Default.UI_ThemeInfo_CustomThemeAccentColor);
        }
        catch (Exception)
        {
            userAccentColor = (Color) ColorConverter.ConvertFromString(Properties.DefaultSettings.Default.UI_ThemeInfo_AccentColor);
        }

        var colorPickerDialog = new ColorPickerDialog(userAccentColor)
        {
            Owner = Application.Current.MainWindow
        };

        if (colorPickerDialog.ShowDialog() != true) return;
        
        var selectedColor = colorPickerDialog.SelectedColor.ToString();
        WhyOrchid.Properties.Settings.Default.Color_Primary = selectedColor;
        WhyOrchid.Properties.Settings.Default.Save();

        Properties.Settings.Default.UI_ThemeInfo_IsCustomAccentColor = true;
        Properties.Settings.Default.UI_ThemeInfo_CustomThemeAccentColor = selectedColor;
        Properties.Settings.Default.Save();

        _uiService.ReloadUi();
    }

    private void Button_ResetAccentColor_OnClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        _colorThemeService.ResetAccentColor();
        _uiService.ReloadUi();
    }

    private void Button_FontPicker_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new FontPicker { Owner = Application.Current.MainWindow };
        if (dialog.ShowDialog() != true) return;
        _uiService.ReloadUi();
    }

    private void InitializeFontRestoreButtonVisibility()
    {
        var mainFont = WhyOrchid.Properties.Settings.Default.FontStyle_FontFamily;
        var indexOfFallback = mainFont.IndexOf(", ", StringComparison.Ordinal);
        if (indexOfFallback != -1)
        {
            mainFont = mainFont.Substring(0, indexOfFallback);
        }

        var isMainFontDefault = mainFont == Properties.DefaultSettings.Default.FontStyle_FontFamily;
        var isFontSizeDefault = Math.Abs(Properties.DefaultSettings.Default.FontStyle_FontSize - WhyOrchid.Properties.Settings.Default.FontStyle_Medium) == 0;

        if (isMainFontDefault && isFontSizeDefault) return;
        ButtonFontRestore.Visibility = Visibility.Visible;
    }

    private void Button_RestoreFont_OnClick(object sender, RoutedEventArgs e)
    {
        WhyOrchid.Properties.Settings.Default.FontStyle_FontFamily = Properties.DefaultSettings.Default.FontStyle_FontFamily;
        WhyOrchid.Properties.Settings.Default.FontStyle_Medium = Properties.DefaultSettings.Default.FontStyle_FontSize;
        WhyOrchid.Properties.Settings.Default.FontStyle_Large  = Properties.DefaultSettings.Default.FontStyle_FontSize + 2;
        WhyOrchid.Properties.Settings.Default.FontStyle_Small  = Properties.DefaultSettings.Default.FontStyle_FontSize - 2;
        WhyOrchid.Properties.Settings.Default.Save();
        _uiService.ReloadUi();
        e.Handled = true;
    }

    private void Button_ResetScale_OnClick(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.UI_Scale = 1;
        Properties.Settings.Default.Save();
        _uiService.ReloadUi();
    }

    private void SliderScale_OnDragCompleted(object sender, DragCompletedEventArgs e)
    {
        Properties.Settings.Default.UI_Scale = SliderScale.Value;
        Properties.Settings.Default.Save();
        _uiService.ReloadUi();
        _isScaleSliderDragged = false;
    }

    private void SliderScale_OnDragStarted(object sender, DragStartedEventArgs e)
    {
        _isScaleSliderDragged = true;
    }

    private void SliderScale_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_isScaleSliderDragged) return;

        Properties.Settings.Default.UI_Scale = SliderScale.Value;
        Properties.Settings.Default.Save();
        _uiService.ReloadUi();
    }

    private void ComboBoxCursorOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var appliedCursorType = (EAppCursor) ((ComboBoxItem)ComboBoxCursor.SelectedItem).Tag;

        var stringCursor = appliedCursorType == EAppCursor.Arrow
            ? Enum.GetName(typeof(EAppCursor), EAppCursor.Arrow)
            : Enum.GetName(typeof(EAppCursor), EAppCursor.Hand);

        WhyOrchid.Properties.Settings.Default.UI_CursorType = stringCursor;
        WhyOrchid.Properties.Settings.Default.Save();
        _uiService.ReloadUi();
    }

    #region Cleanup
    private void AppearancePage_OnUnloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= AppearancePage_OnUnloaded;
        SliderScale.ValueChanged -= SliderScale_OnValueChanged;
        ComboBoxColorTheme.SelectionChanged -= ComboBox_ColorTheme_OnSelectionChanged;
        ComboBoxCursor.SelectionChanged -= ComboBoxCursorOnSelectionChanged;
    }
    #endregion
}
