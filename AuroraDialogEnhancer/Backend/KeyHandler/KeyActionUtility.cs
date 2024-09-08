using System;
using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class KeyActionUtility : IDisposable
{
    private readonly KeyBindingProfileService _keyBindingProfileService;
    private readonly ProcessDataProvider      _processDataProvider;

    public bool                      IsHookPause;
    public KeyBindingProfile         KeyBindingProfile = new();
    public Dictionary<string, Point> InteractionPoints = new(0);
    public List<Rectangle>           DialogOptions     = new(0);

    public object MouseClickLock { get; } = new();
    public bool   IsPrimaryMouseButtonSuspended;
    public Point  PrimaryMouseButtonDownPoint;
    public Point  PrimaryMouseButtonUpPoint;

    public KeyActionUtility(KeyBindingProfileService keyBindingProfileService, 
                            ProcessDataProvider      processDataProvider)
    {
        _keyBindingProfileService = keyBindingProfileService;
        _processDataProvider      = processDataProvider;
    }

    public void InitializeProfile()
    {
        KeyBindingProfile = _keyBindingProfileService.Get(_processDataProvider.Data!.ExtensionConfig!.Id);
    }

    public void Dispose()
    {
        InteractionPoints.Clear();
        DialogOptions.Clear();
        KeyBindingProfile             = new KeyBindingProfile();
        IsPrimaryMouseButtonSuspended = false;
        PrimaryMouseButtonDownPoint   = Point.Empty;
        PrimaryMouseButtonUpPoint     = Point.Empty;
        IsHookPause                   = false;
    }
}
