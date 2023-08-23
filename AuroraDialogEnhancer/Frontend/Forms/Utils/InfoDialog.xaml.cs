using System.Windows;

namespace AuroraDialogEnhancer.Frontend.Forms.Utils;

public partial class InfoDialog : Window
{
    public InfoDialog(InfoDialogConfig config)
    {
        InitializeComponent();

        Owner = config.Owner;

        Title = config.WindowTitle;

        if (!config.ShowSecondaryButton) ButtonSecondary.Visibility = Visibility.Hidden;
        ButtonPrimaryContent.Text = config.PrimaryButtonText;
        ButtonSecondary.Content   = config.SecondaryButtonText;

        TextBoxMessage.Text = config.Message;
        if (config.IsReadOnly)
        {
            TextBoxMessage.Focusable  = false;
            TextBoxMessage.IsHitTestVisible = false;
        }

        if (config.IconData is not null) PathIcon.Data = config.IconData;
        if (config.IconForeground is not null) PathIcon.Foreground = config.IconForeground;
    }

    private void Button_Default_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
