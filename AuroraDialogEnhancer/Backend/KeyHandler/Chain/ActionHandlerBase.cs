using System;

namespace AuroraDialogEnhancer.Backend.KeyHandler.Chain;

public abstract class ActionHandlerBase : IActionHandler
{
    private IActionHandler _nextHandler = new ActionHandlerEmpty();

    public IActionHandler SetNext(IActionHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual bool Handle(Func<bool> request)
    {
        return _nextHandler.Handle(request);
    }
}