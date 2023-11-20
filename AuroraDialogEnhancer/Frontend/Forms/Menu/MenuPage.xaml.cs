using System;
using System.Linq;
using System.Windows;
using AuroraDialogEnhancer.AppConfig.Updater;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Forms.Menu;

public partial class MenuPage
{
    private readonly AutoUpdaterService _autoUpdaterService;

    public Action<EPageType>? MenuItemChecked;

    public MenuPage(AutoUpdaterService autoUpdaterService)
    {
        _autoUpdaterService = autoUpdaterService;
        InitializeComponent();

        Unloaded += MenuPage_Unloaded;
        _autoUpdaterService.OnUpdateAvailabilityChanged += OnUpdateAvailabilityChanged;

#if DEBUG
        ContainerWip.Visibility = Visibility.Visible;
#endif
    }

    private void OnUpdateAvailabilityChanged(object sender, bool e)
    {
        UpdateNotifyIcon.Visibility = e ? Visibility.Visible : Visibility.Collapsed;
    }

    public void SetCheckedItem(EPageType pageType)
    {
        var targetButton = PanelMenuItems.Children.OfType<MenuButton>().FirstOrDefault(button => (EPageType) button.Tag == pageType);
        if (targetButton is null) return;
        targetButton.IsChecked = true;
    }

    private void MenuItem_OnChecked(object sender, RoutedEventArgs e) => MenuItemChecked?.Invoke((EPageType) ((MenuButton) sender).Tag);

    private void MenuPage_Unloaded(object sender, RoutedEventArgs e)
    {
        _autoUpdaterService.OnUpdateAvailabilityChanged -= OnUpdateAvailabilityChanged;
        Unloaded -= MenuPage_Unloaded;
    }
}
