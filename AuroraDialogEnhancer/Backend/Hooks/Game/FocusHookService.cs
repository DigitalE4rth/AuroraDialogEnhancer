using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Backend.Hooks.Game;

public class FocusHookService : IDisposable
{
    private          IApplicationFocusService _applicationFocusService;
    private readonly ProcessDataProvider      _processDataProvider;

    public FocusHookService(FocusHookDefaultService focusHookDefaultService,
                            ProcessDataProvider     processDataProvider)
    {
        _applicationFocusService = focusHookDefaultService;
        _processDataProvider     = processDataProvider;
    }

    public void Initialize()
    {
        if (_processDataProvider.Id != null && _processDataProvider.Id.Equals(Properties.DefaultSettings.Default.App_GenshinImpactExtensionId, StringComparison.Ordinal))
        {
            _applicationFocusService = AppServices.ServiceProvider.GetRequiredService<FocusHookGiService>();
            return;
        }

        _applicationFocusService = AppServices.ServiceProvider.GetRequiredService<FocusHookDefaultService>();
    }

    public event EventHandler<bool>? OnFocusChanged
    {
        add    => _applicationFocusService.OnFocusChanged += value;
        remove => _applicationFocusService.OnFocusChanged -= value;
    }

    public bool IsFocused => _applicationFocusService.IsFocused;

    public void SetWinEventHook() => _applicationFocusService.SetWinEventHook();

    public void SendFocusedEvent() => _applicationFocusService.SendFocusedEvent();

    public void UnhookWinEvent() => _applicationFocusService.UnhookWinEvent();

    public void Dispose()
    {
        UnhookWinEvent();
    }
}
