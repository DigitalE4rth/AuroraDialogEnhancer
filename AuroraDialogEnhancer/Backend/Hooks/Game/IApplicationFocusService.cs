using System;

namespace AuroraDialogEnhancer.Backend.Hooks.Game;

public interface IApplicationFocusService
{
    public event EventHandler<bool>? OnFocusChanged;
    public bool  IsFocused { get; }
    public void  SetWinEventHook();
    public void  SendFocusedEvent();
    public void  UnhookWinEvent();
}
