using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Forms.Menu;

public partial class MenuPage : Page
{
    public Action<EPageType>? OnMenuItemChecked;

    public MenuPage()
    {
        InitializeComponent();
    }

    public void SetCheckedItem(EPageType pageType)
    {
        var targetButton = PanelMenuItems.Children.OfType<MenuButton>().FirstOrDefault(button => (EPageType) button.Tag == pageType);
        if (targetButton is null) return;
        targetButton.IsChecked = true;
    }

    private void MenuItem_OnChecked(object sender, RoutedEventArgs e) => OnMenuItemChecked?.Invoke((EPageType) ((MenuButton) sender).Tag);
}
