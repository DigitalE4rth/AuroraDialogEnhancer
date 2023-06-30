using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.FontPicker;

public partial class FontPicker : Window
{
    private readonly FontPickerDataContext _fontPickerDataContext;
    private const string DefaultSystemFont = "Segoe UI";

    public FontPicker()
    {
        _fontPickerDataContext = new FontPickerDataContext();
        DataContext = _fontPickerDataContext;

        InitializeComponent();

        InitializeFontSelector();
        InitializeTextElementSelectedFont();
    }

    #region Font selector
    private void InitializeFontSelector()
    {
        var systemFontFamilies = new System.Drawing.Text.InstalledFontCollection().Families;
        FontSelector.ItemsSource = systemFontFamilies.Select(ff => new FontFamily(ff.Name));

        var appliedFont = string.IsNullOrEmpty(WhyOrchid.Properties.Settings.Default.FontStyle_FontFamily) ? DefaultSystemFont : WhyOrchid.Properties.Settings.Default.FontStyle_FontFamily;
        var separatorIndex = WhyOrchid.Properties.Settings.Default.FontStyle_FontFamily.IndexOf(", ", StringComparison.Ordinal);
        if (separatorIndex != -1)
        {
            appliedFont = appliedFont.Substring(0, separatorIndex);
        }

        var targetFont = FontSelector.Items.Cast<FontFamily>().FirstOrDefault(ff => ff.Source.Equals(appliedFont, StringComparison.InvariantCulture));
        if (targetFont is null) return;

        FontSelector.SelectedItem = targetFont;
        FontSelector.ScrollIntoView(targetFont);
        TextElementSelectedFont.Text = appliedFont;

        FontSelector.SelectionChanged += FontSelector_SelectionChanged;
    }

    private void FontSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TextElementSelectedFont.TextChanged -= TextElementSelectedFont_OnTextChanged;
        TextElementSelectedFont.Text = ((FontFamily)FontSelector.SelectedItem).Source;
        TextElementSelectedFont.TextChanged += TextElementSelectedFont_OnTextChanged;
    }

    private void InitializeTextElementSelectedFont()
    {
        TextElementSelectedFont.TextChanged += TextElementSelectedFont_OnTextChanged;
    }

    private void TextElementSelectedFont_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var targetFont = FontSelector.Items.Cast<FontFamily>().FirstOrDefault(ff => ff.Source.ToLowerInvariant().Contains(TextElementSelectedFont.Text.ToLowerInvariant()));
        if (targetFont is null) return;

        FontSelector.SelectionChanged -= FontSelector_SelectionChanged;
        FontSelector.SelectedItem = targetFont;
        FontSelector.ScrollIntoView(targetFont);
        FontSelector.SelectionChanged += FontSelector_SelectionChanged;
    }
    #endregion


    #region Size controll buttons
    private void Button_DecreaseFontSize_OnClick(object sender, RoutedEventArgs e)
    {
        if (_fontPickerDataContext.FontSize <= 8) return;
        _fontPickerDataContext.FontSize -= 1;
    }

    private void Button_IncreaseFontSize_OnClick(object sender, RoutedEventArgs e)
    {
        if (_fontPickerDataContext.FontSize >= 24) return;
        _fontPickerDataContext.FontSize += 1;
    }
    #endregion


    #region Confirmation
    private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Button_Apply_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedFont = ((FontFamily)FontSelector.SelectedItem).Source;
        WhyOrchid.Properties.Settings.Default.FontStyle_FontFamily = string.Join(", ", selectedFont, DefaultSystemFont);
        WhyOrchid.Properties.Settings.Default.FontStyle_Medium = _fontPickerDataContext.FontSize;
        WhyOrchid.Properties.Settings.Default.FontStyle_Large  = _fontPickerDataContext.FontSize + 2;
        WhyOrchid.Properties.Settings.Default.FontStyle_Small  = _fontPickerDataContext.FontSize - 2;
        WhyOrchid.Properties.Settings.Default.Save();

        DialogResult = true;
        Close();
    }
    #endregion


    #region Cleanup
    private void FontPicker_OnUnloaded(object sender, RoutedEventArgs e)
    {
        FontSelector.SelectionChanged -= FontSelector_SelectionChanged;
        TextElementSelectedFont.TextChanged -= TextElementSelectedFont_OnTextChanged;
    }
    #endregion
}
