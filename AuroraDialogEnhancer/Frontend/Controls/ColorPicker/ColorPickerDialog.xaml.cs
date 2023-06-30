using System.Windows;
using System.Windows.Media;
using AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Controls;
using AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Formatters;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker;

public partial class ColorPickerDialog : Window
{
    private readonly ColorPickerViewModel _colorPickerViewModel;
    private readonly HexValueFormatter _hexValueFormatter;

    public Color SelectedColor => _colorPickerViewModel.Color;

    public ColorPickerDialog() : this(Colors.Red)
    {
    }

    public ColorPickerDialog(Color color)
    {
        InitializeComponent();

        _hexValueFormatter = new HexValueFormatter();
        _colorPickerViewModel = new ColorPickerViewModel
        {
            Color = color
        };

        DataContext = _colorPickerViewModel;
        InitializeSelectedColorType();
    }

    private void InitializeSelectedColorType()
    {
        var colorType = (EColorType)Properties.Settings.Default.UI_ThemeInfo_SelectedColorType;
        switch (colorType)
        {
            case EColorType.AHEX:
                TextBlockColorSwitcher.Text = "AHEX";
                ContainerHexaInput.Visibility = Visibility.Visible;
                break;
            case EColorType.RGBA:
                TextBlockColorSwitcher.Text = "RGBA";
                ContainerRgbaInput.Visibility = Visibility.Visible;
                break;
            case EColorType.HSBA:
                TextBlockColorSwitcher.Text = "HSBA";
                ContainerHsbInput.Visibility = Visibility.Visible;
                break;
        }
    }

    private void Button_ColorSwitcher_OnClick(object sender, RoutedEventArgs e)
    {
        var colorType = (EColorType)Properties.Settings.Default.UI_ThemeInfo_SelectedColorType;
        switch (colorType)
        {
            case EColorType.AHEX:
                TextBlockColorSwitcher.Text = "RGBA";
                ContainerHexaInput.Visibility = Visibility.Collapsed;
                ContainerRgbaInput.Visibility = Visibility.Visible;
                colorType = EColorType.RGBA;
                break;
            case EColorType.RGBA:
                TextBlockColorSwitcher.Text = "HSB";
                ContainerRgbaInput.Visibility = Visibility.Collapsed;
                ContainerHsbInput.Visibility = Visibility.Visible;
                colorType = EColorType.HSBA;
                break;
            case EColorType.HSBA:
                TextBlockColorSwitcher.Text = "AHEX";
                ContainerHsbInput.Visibility = Visibility.Collapsed;
                ContainerHexaInput.Visibility = Visibility.Visible;
                colorType = EColorType.AHEX;
                break;
        }

        Properties.Settings.Default.UI_ThemeInfo_SelectedColorType = (int)colorType;
        Properties.Settings.Default.Save();
    }

    private void PaletteButton_OnClick(object sender, RoutedEventArgs e)
    {
        _colorPickerViewModel.Color = ((SolidColorBrush)((PaletteButton)sender).Background).Color;
    }

    private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Button_Confirm_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
