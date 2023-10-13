using System;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;

namespace AuroraDialogEnhancer.Backend.Hooks.Mouse;

public class MouseHookManagerRecordService : MouseHookManagerServiceBase
{
    public event EventHandler<EHighMouseKey>? OnMouseKeyDown;

    public void RegisterAllHighLevelKeys()
    {
        RegisterHotKey(EHighMouseKey.MouseWheelUp,   OnWheelUp);
        RegisterHotKey(EHighMouseKey.MouseWheelDown, OnWheelDown);
        RegisterHotKey(EHighMouseKey.MiddleButton,   OnMiddleDown);
        RegisterHotKey(EHighMouseKey.Back,           OnBackDown);
        RegisterHotKey(EHighMouseKey.Forward,        OnForwardDown);
    }

    public override void Stop()
    {
        base.Stop();
        UnRegisterAll();
    }

    private void OnWheelUp()     => OnMouseKeyDown?.Invoke(this, EHighMouseKey.MouseWheelUp);
    private void OnWheelDown()   => OnMouseKeyDown?.Invoke(this, EHighMouseKey.MouseWheelDown);
    private void OnMiddleDown()  => OnMouseKeyDown?.Invoke(this, EHighMouseKey.MiddleButton);
    private void OnBackDown()    => OnMouseKeyDown?.Invoke(this, EHighMouseKey.Back);
    private void OnForwardDown() => OnMouseKeyDown?.Invoke(this, EHighMouseKey.Forward);
}
