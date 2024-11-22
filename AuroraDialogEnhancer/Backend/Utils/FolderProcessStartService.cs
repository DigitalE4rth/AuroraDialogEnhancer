using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.External;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Linq;

namespace AuroraDialogEnhancer.Backend.Utils;

public class FolderProcessStartService
{

    public void Open(string folderPath)
    {
        EnsureDirectory(folderPath);
        if (ShowFolderIfOpened(folderPath)) return;
        StartProcess(folderPath);
    }

    private void EnsureNoEndTrailing(ref string folderPath)
    {
        if (!folderPath.EndsWith("\\")) return;
        folderPath = folderPath.Remove(folderPath.Length-1, 1);
    }

    public void EnsureDirectory(string folderPath)
    {
        if (Directory.Exists(folderPath)) return;
        Directory.CreateDirectory(folderPath);
    }

    public bool ShowFolderIfOpened(string folderPath)
    {
        EnsureNoEndTrailing(ref folderPath);
        var process = Process.GetProcesses().FirstOrDefault(process =>
            process.ProcessName.Equals(AppConstants.OsEnvironment.ExplorerProcessName) && process.MainWindowTitle.Equals(folderPath));

        if (process == null || process.MainWindowHandle == IntPtr.Zero) return false;

        WinApi.ShowWindowAsync(process.MainWindowHandle, EShowWindowMode.SW_RESTORE);
        WinApi.SetForegroundWindow(process.MainWindowHandle);
        return true;
    }

    public void StartProcess(string arguments)
    {
        Process.Start(new ProcessStartInfo
        {
            Arguments = arguments,
            UseShellExecute = true,
            FileName = AppConstants.OsEnvironment.ExplorerName
        });
    }
}
