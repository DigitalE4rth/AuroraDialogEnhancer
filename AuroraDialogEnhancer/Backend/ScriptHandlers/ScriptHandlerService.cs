using System;

namespace AuroraDialogEnhancer.Backend.ScriptHandlers;

public class ScriptHandlerService : IDisposable
{
    public readonly AutoClickScript AutoClickScript;

    public ScriptHandlerService(AutoClickScript autoClickScript)
    {
        AutoClickScript = autoClickScript;
    }

    public void UnRegisterAll() => AutoClickScript.UnRegister();

    public void Dispose()
    {
        AutoClickScript.Dispose();
    }
}
