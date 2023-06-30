using System.Windows;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var appConfig = AppServices.ServiceProvider.GetRequiredService<AppConfig.Config.AppConfig>();
        if (!appConfig.Initialize(e)) return;
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        AppServices.ServiceProvider.GetRequiredService<AppConfig.Config.AppConfig>().Dispose();
        base.OnExit(e);
    }
}
