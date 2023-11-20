using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AuroraDialogEnhancer.AppConfig.Updater;
using AuroraDialogEnhancer.Backend.Extensions;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Forms.About;

public partial class AboutPage
{
    private readonly ExtensionsProvider _extensionsProvider;
    private readonly AutoUpdaterService _autoUpdaterService;

    public AboutPage(AutoUpdaterService autoUpdaterService, ExtensionsProvider extensionsProvider)
    {
        Unloaded += OnUnloaded;
        _autoUpdaterService = autoUpdaterService;
        _extensionsProvider = extensionsProvider;
        InitializeComponent();
        // ToDo: Enable/Rewrite for two or more extensions. One extension doesn't look pretty in UI
        //InitializeExtensions();
        InitializeComboBoxUpdateFrequency();
        _autoUpdaterService.OnUpdateAvailabilityChanged += OnUpdateAvailabilityChanged;
    }

    private void InitializeExtensions()
    {
        var isLeft = true;
        var leftExtensions = new List<ExtensionViewModel>();
        var rightExtensions = new List<ExtensionViewModel>();
        foreach (var extension in _extensionsProvider.ExtensionsDictionary.Values)
        {
            if (isLeft)
            {
                leftExtensions.Add(new ExtensionViewModel(extension));
                isLeft = false;
                continue;
            }

            rightExtensions.Add(new ExtensionViewModel(extension));
            isLeft = true;
        }

        ContainerExtensionsLeft.ItemsSource = leftExtensions;
        ContainerExtensionsRight.ItemsSource = rightExtensions;
    }

    private void InitializeComboBoxUpdateFrequency()
    {
        var item = ComboBoxUpdateFrequency.Items.OfType<ComboBoxItem>().FirstOrDefault(item => (int)item.Tag == Properties.Settings.Default.Update_Frequency);
        if (item is not null)
        {
            ComboBoxUpdateFrequency.SelectedItem = item;
            PathIconUpdateFrequency.Data = ((EUpdateFrequency)item.Tag == EUpdateFrequency.None)
                ? (PathGeometry) Application.Current.Resources["I.R.NotificationsOff"]
                : (PathGeometry) Application.Current.Resources["I.R.Notifications"];
        }
        
        ComboBoxUpdateFrequency.SelectionChanged += ComboBoxUpdateFrequency_SelectionChanged;
    }

    private void ComboBoxUpdateFrequency_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var updateFrequency = (EUpdateFrequency)((ComboBoxItem)ComboBoxUpdateFrequency.SelectedItem).Tag;

        Properties.Settings.Default.Update_Frequency = (int) updateFrequency;
        Properties.Settings.Default.Save();

        PathIconUpdateFrequency.Data = updateFrequency == EUpdateFrequency.None
            ? (PathGeometry)Application.Current.Resources["I.R.NotificationsOff"]
            : (PathGeometry)Application.Current.Resources["I.R.Notifications"];
    }

    private void OnUpdateAvailabilityChanged(object sender, bool e)
    {
        ContainerUpdateIcon.Visibility = e ? Visibility.Visible: Visibility.Collapsed;
    }

    private void Button_Link_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start((string) ((CardButton) sender).Tag);
    }

    private void Button_Update_OnClick(object sender, RoutedEventArgs e)
    {
        _autoUpdaterService.CheckForUpdateManual();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= OnUnloaded;
        ComboBoxUpdateFrequency.SelectionChanged -= ComboBoxUpdateFrequency_SelectionChanged;
        _autoUpdaterService.OnUpdateAvailabilityChanged -= OnUpdateAvailabilityChanged;
    }
}

