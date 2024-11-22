using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.External;
using AuroraDialogEnhancer.Backend.Hooks.Game;

namespace AuroraDialogEnhancer.Backend.Hooks.Process;

public class ProcessInfoService
{
    private readonly ProcessDataProvider _processDataProvider;

    public ProcessInfoService(ProcessDataProvider processDataProvider)
    {
        _processDataProvider = processDataProvider;
    }

    public async Task StartAndDetectProcessAsync(ExtensionConfig extensionConfig, CancellationTokenSource cancellationTokenSource)
    {
        StartProcess(extensionConfig);
        ApplyProcessInfo(extensionConfig);

        if (_processDataProvider.IsGameProcessAlive()) return;

        while (!_processDataProvider.IsGameProcessAlive())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(5000), cancellationTokenSource.Token);
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            ApplyProcessInfo(extensionConfig);
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
        }
    }

    public void StartProcess(ExtensionConfig extensionConfig)
    {
        if (extensionConfig.HookLaunchType == EHookLaunchType.Nothing) return;

        var targetProcessName = extensionConfig.HookLaunchType == EHookLaunchType.Game ? extensionConfig.GameProcessName : extensionConfig.LauncherProcessName;
        var targetProcessLocation = extensionConfig.HookLaunchType == EHookLaunchType.Game ? extensionConfig.GameLocation : extensionConfig.LauncherLocation;

        var isProcessPresent = IsProcessPresent(targetProcessName, targetProcessLocation);
        if (isProcessPresent || string.IsNullOrEmpty(targetProcessLocation)) return;
        System.Diagnostics.Process.Start(new ProcessStartInfo
        {
            Arguments = targetProcessLocation,
            UseShellExecute = true,
            FileName = AppConstants.OsEnvironment.ExplorerName
        });
    }

    /// <summary>
    /// Concrete process detection method.
    /// </summary>
    /// <returns>Detected <see cref="ProcessInfo"/> or <see langword="null"/> if not found.</returns>
    public void ApplyProcessInfo(ExtensionConfig extensionConfig)
    {
        var targetProcess = GetProcess(extensionConfig.GameProcessName, extensionConfig.GameLocation);
        if (targetProcess is null) return;

        targetProcess.EnableRaisingEvents = true;
        _processDataProvider.Data = new HookedGameData
        {
            ExtensionConfig = extensionConfig,
            GameProcess = targetProcess
        };
        ApplyMainWindowInfo(targetProcess.MainWindowHandle);
    }

    public bool IsProcessPresent(string processName, string location) => GetProcess(processName, location) is not null;

    public System.Diagnostics.Process? GetProcess(int processId)
    {
        return System.Diagnostics.Process.GetProcesses().FirstOrDefault(process => process.Id == processId);
    }

    private System.Diagnostics.Process? GetProcess(string processName, string location = "")
    {
        var processes = System.Diagnostics.Process.GetProcesses();
        var filteredProcesses = processes.Where(process => process.ProcessName.Equals(processName, StringComparison.Ordinal));

        var targetProcess = location == string.Empty 
            ? filteredProcesses.FirstOrDefault()
            : filteredProcesses.FirstOrDefault(process =>
        {
            var processLocation = GetProcessLocation(process);
            return processLocation is not null && processLocation.Equals(location, StringComparison.Ordinal);
        });

        if (targetProcess is null || targetProcess.MainWindowHandle == IntPtr.Zero) return null;
        var mainWindowSize = GetWindowRectangle(targetProcess.MainWindowHandle);
        return mainWindowSize == Rectangle.Empty ? null : targetProcess;
    }

    // Access Denied permission bypass, while accessing x32 app from x64 app and vise versa.
    public string? GetProcessLocation(System.Diagnostics.Process process)
    {
        var capacity = 4000;
        var builder = new StringBuilder(capacity);
        var ptr = WinApi.OpenProcess(WinApi.ProcessAccessFlags.QueryLimitedInformation, false, process.Id);
        var result = !WinApi.QueryFullProcessImageName(ptr, 0, builder, ref capacity) ? null : builder.ToString();
        WinApi.CloseHandle(ptr);
        return result;
    }

    private IEnumerable<System.Diagnostics.Process> GetProcessesByUserParams(Dictionary<int, string> processNamesDict)
    {
        var processes = System.Diagnostics.Process.GetProcesses();
        return FilterByProcessName(processes, processNamesDict.Values);
    }

    private IEnumerable<System.Diagnostics.Process> GetProcessesByUserParams(ExtensionConfig extensionConfig)
    {
        var processes = System.Diagnostics.Process.GetProcesses();
        return FilterByProcessName(processes, new List<string>(1) { extensionConfig.GameProcessName });
    }

    private void ApplyMainWindowInfo(IntPtr mainWindowHandle)
    {
        var clientRectangle = GetClientRectangle(mainWindowHandle);
        var windowRectangle = GetWindowRectangle(mainWindowHandle);

        _processDataProvider.Data!.GameWindowInfo = new WindowInfo(mainWindowHandle, clientRectangle, windowRectangle);
    }

    /// <summary>
    /// Filters the collection of provided processes by the user-specified process name.
    /// </summary>
    /// <remarks>
    /// Supports various search patterns (with Invariant Culture and Case Matching) for the following characters at the beginning of the string:
    /// <list type="bullet">
    /// <item><description>'^' - Starts with.</description></item>
    /// <item><description>'&amp;' - Ends with.</description></item>
    /// <item><description>'*' - Contains.</description></item>
    /// <item><description>none - Equals.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="processes">A collection of processes.</param>
    /// <param name="processNames">Names of the processes to search for.</param>
    /// <returns>Filtered processes.</returns>
    private IEnumerable<System.Diagnostics.Process> FilterByProcessName(System.Diagnostics.Process[] processes, IEnumerable<string> processNames)
    {
        return from process in processes
               from processName in processNames
               where process.ProcessName.Contains(processName)
               select process;
    }

    /// <summary>
    /// Filters the collection of provided processes by the user-specified window title.
    /// </summary>
    /// <remarks>
    /// Supports various search patterns (with Invariant Culture and Case Matching) for the following characters at the beginning of the string:
    /// <list type="bullet">
    /// <item><description>'^' - Starts with.</description></item>
    /// <item><description>'&amp;' - Ends with.</description></item>
    /// <item><description>'*' - Contains.</description></item>
    /// <item><description>none - Equals.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="processes">A collection of processes.</param>
    /// <param name="windowTitle">Title of the window to search for.</param>
    /// <returns>Filtered processes.</returns>
    private IEnumerable<System.Diagnostics.Process> FilterByWindowTitle(IEnumerable<System.Diagnostics.Process> processes, string windowTitle)
    {
        if (windowTitle.StartsWith("^"))
        {
            windowTitle = windowTitle.Substring(1, windowTitle.Length - 1);
            return processes.Where(process => process.MainWindowTitle.StartsWith(windowTitle));
        }

        if (windowTitle.EndsWith("&"))
        {
            windowTitle = windowTitle.Substring(0, windowTitle.Length - 1);
            return processes.Where(process => process.MainWindowTitle.EndsWith(windowTitle));
        }

        if (windowTitle.StartsWith("*"))
        {
            windowTitle = windowTitle.Substring(1, windowTitle.Length - 1);
            return processes.Where(process => process.MainWindowTitle.Contains(windowTitle));
        }

        return processes.Where(process => process.MainWindowTitle.Equals(windowTitle, StringComparison.InvariantCulture));
    }

    /// <summary>
    /// Filters the collection of provided processes by the existence of any window within.
    /// </summary>
    /// <param name="processes">A collection of processes.</param>
    /// <returns>Filtered processes.</returns>
    private IEnumerable<System.Diagnostics.Process> FilterByWindowExistence(IEnumerable<System.Diagnostics.Process> processes)
    {
        return processes.Where(process => process.MainWindowHandle != IntPtr.Zero);
    }

    public void SetWindowLocation()
    {
        var currentWindowRectangle = _processDataProvider.Data!.GameWindowInfo!.WindowRectangle;
        var newWindowRectangle = GetWindowRectangle(_processDataProvider.Data!.GameProcess!.MainWindowHandle);
        var newClientRectangle = GetClientRectangle(_processDataProvider.Data!.GameProcess!.MainWindowHandle);

        if (currentWindowRectangle != newWindowRectangle)
        {
            _processDataProvider.Data!.GameWindowInfo.SetLocation(newClientRectangle, newWindowRectangle);
        }
    }

    public void ApplyWindowInfo()
    {
        var clientRectangle = GetClientRectangle(_processDataProvider.Data!.GameProcess!.MainWindowHandle);
        var windowRectangle = GetWindowRectangle(_processDataProvider.Data!.GameProcess!.MainWindowHandle);

        _processDataProvider.Data!.GameWindowInfo = new WindowInfo(_processDataProvider.Data!.GameProcess!.MainWindowHandle, clientRectangle, windowRectangle);
    }

    /// <summary>
    /// Detects window client rectangle.
    /// </summary>
    /// <param name="intPtr">Target window handle pointer.</param>
    /// <returns>Target window client rectangle.</returns>
    private Rectangle GetClientRectangle(IntPtr intPtr)
    {
        WinApi.GetClientRect(intPtr, out var clientRectangle);
        return clientRectangle;
    }

    /// <summary>
    /// Detects window rectangle.
    /// </summary>
    /// <param name="intPtr">Target window handle pointer.</param>
    /// <returns>Target window rectangle.</returns>
    private Rectangle GetWindowRectangle(IntPtr intPtr)
    {
        var windowRectangle = new Rectangle();
        WinApi.GetWindowRect(intPtr, ref windowRectangle);
        return windowRectangle;
    }
}
