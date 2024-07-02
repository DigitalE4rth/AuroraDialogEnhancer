using System;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Backend.Hooks.Game;

public class FocusHookServiceFactory
{
    public IGameFocusService Get(string? gameId = "")
    {
        return gameId != null && gameId.Equals("GI", StringComparison.Ordinal) 
            ? AppServices.ServiceProvider.GetRequiredService<FocusHookGameGiService>() 
            : AppServices.ServiceProvider.GetRequiredService<FocusHookGameService>();
    }
}
