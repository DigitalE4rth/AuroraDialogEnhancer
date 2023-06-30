using System.Diagnostics;
using AuroraDialogEnhancer.AppConfig.Statics;

namespace AuroraDialogEnhancer.Backend.Hooks.Process;

public class ProcessStartService
{
    public void StartProcess(string path)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName  = Global.StringConstants.ExplorerName,
            Arguments = path
        };

        var process = System.Diagnostics.Process.Start(processStartInfo);
        process?.WaitForExit(0);
    }
}
