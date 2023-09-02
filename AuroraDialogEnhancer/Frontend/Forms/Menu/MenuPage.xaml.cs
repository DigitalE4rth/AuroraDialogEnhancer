using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Forms.Menu;

public partial class MenuPage
{
    public Action<EPageType>? MenuItemChecked;

    public MenuPage()
    {
        InitializeComponent();

#if DEBUG
        ContainerWip.Visibility = Visibility.Visible;
#endif
    }

    public void SetCheckedItem(EPageType pageType)
    {
        var targetButton = PanelMenuItems.Children.OfType<MenuButton>().FirstOrDefault(button => (EPageType) button.Tag == pageType);
        if (targetButton is null) return;
        targetButton.IsChecked = true;
    }

    private void MenuItem_OnChecked(object sender, RoutedEventArgs e) => MenuItemChecked?.Invoke((EPageType) ((MenuButton) sender).Tag);
}
