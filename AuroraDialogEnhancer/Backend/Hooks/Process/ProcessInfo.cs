namespace AuroraDialogEnhancer.Backend.Hooks.Process;

public class ProcessInfo
{
    public ProcessInfo(System.Diagnostics.Process process)
    {
        Process = process;
        Process.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Target process.
    /// </summary>
    public System.Diagnostics.Process? Process { get; }
}
