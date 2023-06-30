using System;

namespace AuroraDialogEnhancer.AppConfig.Config;

public sealed class SingleInstanceEventArgs : EventArgs
{
    public SingleInstanceEventArgs(int processId, string[] arguments)
    {
        ProcessId = processId;
        Arguments = arguments;
    }

    public int ProcessId { get; }

    public string[] Arguments { get; }
}
