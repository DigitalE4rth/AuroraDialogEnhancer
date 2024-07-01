using System;

namespace AuroraDialogEnhancer.Backend.Hooks.Game;

public class ProcessDataProvider : IDisposable
{
    public EHookState HookState { get; private set; } = EHookState.None;

    public string? Id { get; set; }
    
    public string Message { get; private set; } = string.Empty;

    public HookedGameData? Data { get; set; }

    public void SetStateAndNotify(EHookState state, string message)
    {
        HookState = state;
        Message = message;
        OnHookStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetStateAndNotify(EHookState state) => SetStateAndNotify(state, string.Empty);

    public event EventHandler? OnHookStateChanged;

    public bool IsExtenstionConfigPresent() => Data?.ExtensionConfig != null;

    public bool IsGameProcessAlive() => Data?.GameProcess != null;

    public void Dispose()
    {
        Data?.Dispose();
        Data = null;
        HookState = EHookState.None;
        Message = string.Empty;
        Id = null;
    }
}
