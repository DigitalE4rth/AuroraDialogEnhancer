using System;

namespace AuroraDialogEnhancer.Backend.KeyHandler.Chain;

public class ActionHandlerEmpty : ActionHandlerBase
{
    public override bool Handle(Func<bool> request) => false;
}