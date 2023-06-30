using System.Windows;

namespace AuroraDialogEnhancer.Frontend.Forms.Utils;

public partial class TextInputResetDialog
{
    public string? Result { get; private set; }

    public TextInputResetDialog(TextInputResetDialogDataContext config)
    {
        DataContext = config;

        InitializeComponent();

        if (config.ResetContent is not null) ButtonReset.Visibility = Visibility.Visible;
        if (config.TextBoxTitle is null) TextBlockElementTitle.Visibility = Visibility.Collapsed;
        TextBoxContent.SelectAll();
        TextBoxContent.Focus();
    }

    private void Button_Default_OnClick(object sender, RoutedEventArgs e)
    {
        Result = ((TextInputResetDialogDataContext) DataContext).TextContent;
        DialogResult = true;
    }

    private void Button_Cancel_OnClick(object sender, RoutedEventArgs e) => DialogResult = false;

    private void Button_Reset_OnClick(object sender, RoutedEventArgs e) 
    {
        var config = (TextInputResetDialogDataContext) DataContext;
        config.TextContent  = config.ResetContent;
        TextBoxContent.Text = config.ResetContent;
    }
}
