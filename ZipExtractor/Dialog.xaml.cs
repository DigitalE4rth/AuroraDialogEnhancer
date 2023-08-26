using System;
using System.Windows;

namespace Updater;

public partial class Dialog : Window
{
    public Dialog()
    {
        InitializeComponent();
    }

    [STAThread]
    public override void BeginInit()
    {
        base.BeginInit();
    }

    public void Initialize(Window owner, string text, string caption, string confirmButtonText, string cancelButtonText)
    {
        Owner = owner;
        TextInfo.Text = text;
        Title = caption;
        ButtonConfirm.Content = confirmButtonText;
        ButtonCancel.Content = cancelButtonText;

        if (string.IsNullOrEmpty(confirmButtonText))
        {
            ButtonConfirm.Visibility = Visibility.Collapsed;
        }

        if (string.IsNullOrEmpty(cancelButtonText))
        {
            ButtonCancel.Visibility = Visibility.Collapsed;
        }

    }

    private void ButtonConfirm_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}

