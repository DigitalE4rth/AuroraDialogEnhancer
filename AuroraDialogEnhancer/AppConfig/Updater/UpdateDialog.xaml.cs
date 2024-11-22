using System;
using System.Diagnostics;
using System.Windows;
using AuroraDialogEnhancer.AppConfig.Statics;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public partial class UpdateDialog
{
    private UpdateInfo _updateInfo = new();

    public UpdateDialog()
    {
        InitializeComponent();
        Closing += UpdateWindow_Closing;
    }

    public void Initialize(UpdateInfo updateInfo)
    {
        _updateInfo = updateInfo;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        InitializeUpdateInfo();
    }

    private void InitializeUpdateInfo()
    {
        if (_updateInfo.IsUpdateAvailable)
        {
            ShowNewVersionInfo();
            return;
        }

        ShowCurrentVersionInfo();
    }

    private void ShowCurrentVersionInfo()
    {
        TextNewVersion.Visibility  = Visibility.Collapsed;
        TextCurrentVersion.Text    = string.Format(Properties.Localization.Resources.AutoUpdate_CurrentIsLatestVersion, AppConstants.AssemblyInfo.VersionString);
        GridChangelog.Visibility   = Visibility.Collapsed;
        ButtonSecondary.Visibility = Visibility.Hidden;
        ButtonPrimary.Content      = Properties.Localization.Resources.AutoUpdate_Close;
        ButtonPrimary.Click       += Button_Default_Close_OnClick;
    }

    private void ShowNewVersionInfo()
    {
        TextNewVersion.Text        = string.Format(Properties.Localization.Resources.AutoUpdate_NewVersionAvailable, _updateInfo.Version);
        TextCurrentVersion.Text    = string.Format(Properties.Localization.Resources.AutoUpdate_CurrentVersion, AppConstants.AssemblyInfo.VersionString);
        TextBlockHyperlink.ToolTip = _updateInfo.ChangelogUri;
        ButtonPrimary.Content      = Properties.Localization.Resources.AutoUpdate_Update_Verb;
        ButtonPrimary.Click       += Button_Default_Update_OnClick;
    }

    private void HyperlinkChangelog_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start(_updateInfo.ChangelogUri);
    }

    private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void Button_Default_Update_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Button_Default_Close_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void UpdateWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Closing -= UpdateWindow_Closing;
        ButtonPrimary.Click -= Button_Default_Update_OnClick;
        ButtonPrimary.Click -= Button_Default_Close_OnClick;
    }
}
