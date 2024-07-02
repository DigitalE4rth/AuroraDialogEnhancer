using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using System;

namespace AuroraDialogEnhancer.Backend.KeyHandler.Chain;

internal class CanBeExecutedAction : ActionHandlerBase
{
    private readonly CursorVisibilityStateProvider _cursorVisibilityStateProvider;
    private readonly CursorPositioningService      _cursorPositioningService;

    private readonly object _lock = new();
    private bool _isProcessing;

    public CanBeExecutedAction(CursorVisibilityStateProvider cursorVisibilityStateProvider,
                               CursorPositioningService      cursorPositioningService)
    {
        _cursorVisibilityStateProvider = cursorVisibilityStateProvider;
        _cursorPositioningService = cursorPositioningService;
    }

    public override bool Handle(Func<bool> request)
    {
        lock (_lock)
        {
            if (_isProcessing) return false;
            _isProcessing = true;
        }

        var result = base.Handle(request);

        _isProcessing = false;
        return result;
    }
}
