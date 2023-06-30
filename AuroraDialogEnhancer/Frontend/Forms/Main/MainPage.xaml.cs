using System.Windows.Controls;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Frontend.Forms.Menu;
using AuroraDialogEnhancer.Frontend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Frontend.Forms.Main;

public partial class MainPage : Page
{
    private readonly UiService _uiService;

    public MainPage(UiService uiService)
    {
        _uiService = uiService;

        InitializeComponent();

        InitializeUiService();
        InitializeMenuPage();
        InitializeMainPage();
    }

    private void InitializeUiService() => _uiService.InitializeMainFrame(FrameMain);

    private void InitializeMenuPage()
    {
        var menuPage = AppServices.ServiceProvider.GetRequiredService<MenuPage>();
        FrameMenu.NavigationService.Navigate(menuPage);
        FrameMain.NavigationService.RemoveBackEntry();

        menuPage.SetCheckedItem(_uiService.CurrentPage);
        menuPage.OnMenuItemChecked = OnMenuItemChecked;
    }

    private void InitializeMainPage() => _uiService.Navigate(_uiService.CurrentPage);

    private void OnMenuItemChecked(EPageType pageType) => _uiService.Navigate(pageType);
}
