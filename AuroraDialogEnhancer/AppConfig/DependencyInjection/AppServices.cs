using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.AppConfig.DependencyInjection;

internal class AppServices
{
    internal static ServiceProvider ServiceProvider = new ServiceProviderConfigurator().ConfigureServices();
}
