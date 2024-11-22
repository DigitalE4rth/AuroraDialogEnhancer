using System;

namespace AuroraDialogEnhancer.Backend.Hooks.Global;

public interface IFocusHook
{
    public bool IsFocused { get; }
    public event EventHandler<bool>? OnFocusChanged;
    public void SetWinEventHook();
    public void SendFocusedEvent();
    public void UnhookWinEvent();
}
