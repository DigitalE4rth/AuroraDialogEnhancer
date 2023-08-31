using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using Path = System.IO.Path;

namespace Updater;

public partial class MainWindow
{
    private readonly StringBuilder _logBuilder = new();
    private BackgroundWorker? _backgroundWorker;
    private bool _isSuccess;

    public MainWindow()
    {
        Closing += MainWindow_Closing;
        InitializeComponent();
        ExtractZipContent();
    }

    private void ExtractZipContent()
    {
        var zipPath = string.Empty;
        var extractionPath = string.Empty;
        var currentExe = string.Empty;
        var updatedExe = string.Empty;
        var clearAppDirectory = false;
        var commandLineArgs = string.Empty;

        _logBuilder.AppendLine(DateTime.Now.ToString("F"));
        _logBuilder.AppendLine();
        _logBuilder.AppendLine("ZipExtractor started with following command line arguments.");

        string[] args = Environment.GetCommandLineArgs();
        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index].ToLower();
            switch (arg)
            {
                case "--input":
                    zipPath = args[index + 1];
                    break;
                case "--output":
                    extractionPath = args[index + 1];
                    break;
                case "--current-exe":
                    currentExe = args[index + 1];
                    break;
                case "--updated-exe":
                    updatedExe = args[index + 1];
                    break;
                case "--clear":
                    clearAppDirectory = true;
                    break;
                case "--args":
                    commandLineArgs = args[index + 1];
                    break;
            }

            _logBuilder.AppendLine($"[{index}] {arg}");
        }

        _logBuilder.AppendLine();

        if (string.IsNullOrEmpty(zipPath) || string.IsNullOrEmpty(extractionPath) || string.IsNullOrEmpty(currentExe))
        {
            return;
        }

        // Extract all the files.
        _backgroundWorker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        _backgroundWorker.DoWork += (_, eventArgs) =>
        {
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(currentExe)))
            {
                try
                {
                    if (process.MainModule is { FileName: not null } && process.MainModule.FileName.Equals(currentExe))
                    {
                        _logBuilder.AppendLine("Waiting for application process to exit...");

                        _backgroundWorker.ReportProgress(0, "Waiting for application to exit...");
                        process.WaitForExit();
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
            }

            _logBuilder.AppendLine("BackgroundWorker started successfully.");

            // Ensures that the last character on the extraction path
            // is the directory separator char.
            // Without this, a malicious zip file could try to traverse outside of the expected
            // extraction path.
            if (!extractionPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                extractionPath += Path.DirectorySeparatorChar;
            }

            var archive = ZipFile.OpenRead(zipPath);

            ReadOnlyCollection<ZipArchiveEntry> entries = archive.Entries;

            try
            {
                var progress = 0;

                if (clearAppDirectory)
                {
                    _logBuilder.AppendLine($"Removing all files and folders from \"{extractionPath}\".");
                    var directoryInfo = new DirectoryInfo(extractionPath);

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        _logBuilder.AppendLine($"Removing a file located at \"{file.FullName}\".");
                        _backgroundWorker.ReportProgress(0, string.Format(Properties.Resources.Removing, file.FullName));
                        file.Delete();
                    }

                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                        _logBuilder.AppendLine($"Removing a directory located at \"{directory.FullName}\" and all its contents.");
                        _backgroundWorker.ReportProgress(0, string.Format(Properties.Resources.Removing, directory.FullName));
                        directory.Delete(true);
                    }
                }

                _logBuilder.AppendLine($"Found total of {entries.Count} files and folders inside the zip file.");

                for (var index = 0; index < entries.Count; index++)
                {
                    if (_backgroundWorker.CancellationPending)
                    {
                        eventArgs.Cancel = true;
                        break;
                    }

                    var entry = entries[index];

                    var currentFile = string.Format(Properties.Resources.CurrentFileExtracting, entry.FullName);
                    _backgroundWorker.ReportProgress(progress, currentFile);
                    var notCopied = true;
                    while (notCopied)
                    {
                        var filePath = string.Empty;
                        try
                        {
                            filePath = Path.Combine(extractionPath, entry.FullName);
                            if (!entry.IsDirectory())
                            {
                                var parentDirectory = Path.GetDirectoryName(filePath);
                                if (parentDirectory != null)
                                {
                                    if (!Directory.Exists(parentDirectory))
                                    {
                                        Directory.CreateDirectory(parentDirectory);
                                    }
                                }
                                else
                                {
                                    throw new ArgumentNullException($"parentDirectory is null for \"{filePath}\"!");
                                }

                                using (Stream destination = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write,
                                           FileShare.None))
                                {
                                    using var stream = entry.Open();
                                    stream.CopyTo(destination);
                                    destination.SetLength(destination.Position);
                                }

                                File.SetLastWriteTime(filePath, entry.LastWriteTime.DateTime);
                            }

                            notCopied = false;
                        }
                        catch (IOException exception)
                        {
                            const int errorSharingViolation = 0x20;
                            const int errorLockViolation = 0x21;
                            var errorCode = Marshal.GetHRForException(exception) & 0x0000FFFF;
                            if (errorCode is not (errorSharingViolation or errorLockViolation))
                            {
                                throw;
                            }

                            List<Process>? lockingProcesses = null;
                            if (Environment.OSVersion.Version.Major >= 6)
                            {
                                try
                                {
                                    lockingProcesses = FileUtil.WhoIsLocking(filePath);
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }

                            if (lockingProcesses == null)
                            {
                                Thread.Sleep(5000);
                                continue;
                            }

                            foreach (var lockingProcess in lockingProcesses)
                            {
                                var result = true;

                                Application.Current.Dispatcher.Invoke(() => 
                                { 
                                    var errorDialog = new ErrorDialog();
                                    errorDialog.Initialize(this,
                                        $"{lockingProcess.ProcessName} {Properties.Resources.FileStillInUseMessage_1}{Environment.NewLine}{Environment.NewLine}{filePath}{Environment.NewLine}{Environment.NewLine}{Properties.Resources.FileStillInUseMessage_2}",
                                        Properties.Resources.FileStillInUseCaption,
                                        Properties.Resources.Retry,
                                        Properties.Resources.Cancel);

                                    if (errorDialog.ShowDialog() != true)
                                    {
                                        result = false;
                                    }
                                });

                                if (result != true)
                                {
                                    throw;
                                }
                            }
                        }
                    }

                    progress = (index + 1) * 100 / entries.Count;
                    _backgroundWorker.ReportProgress(progress, currentFile);

                    _logBuilder.AppendLine($"{currentFile} [{progress}%]");
                }

                archive.Dispose();
                File.Delete(zipPath);
                _logBuilder.AppendLine("Deleting an update zip file.");

                _isSuccess = true;
            }
            catch (Exception exception)
            {
                _logBuilder.AppendLine($"{exception.Message}{Environment.NewLine}{exception.InnerException?.Message}");
            }
            finally
            {
                archive.Dispose();
            }
        };

        _backgroundWorker.ProgressChanged += (_, eventArgs) =>
        {
            ProgressBar.Value = eventArgs.ProgressPercentage;

            if (eventArgs.UserState == null)
            {
                return;
            }

            TextStatus.Text = eventArgs.UserState.ToString();
            TextStatus.SelectionStart = TextStatus.Text.Length;
            TextStatus.SelectionLength = 0;
        };

        _backgroundWorker.RunWorkerCompleted += (_, eventArgs) =>
        {
            try
            {
                if (eventArgs.Error != null)
                {
                    throw eventArgs.Error;
                }

                if (eventArgs.Cancelled)
                {
                    return;
                }

                TextStatus.Text = @"Finished";

                try
                {
                    var executablePath = string.IsNullOrWhiteSpace(updatedExe) ? currentExe : Path.Combine(extractionPath, updatedExe);
                    var processStartInfo = new ProcessStartInfo(executablePath);
                    if (!string.IsNullOrEmpty(commandLineArgs)) { processStartInfo.Arguments = commandLineArgs; }

                    Process.Start(processStartInfo);

                    _logBuilder.AppendLine("Successfully launched the updated application.");
                }
                catch (Win32Exception exception) 
                {
                    if (exception.NativeErrorCode != 1223)
                    {
                        throw;
                    }
                }
            }
            catch (Exception exception)
            {
                _logBuilder.AppendLine();
                _logBuilder.AppendLine(exception.ToString());

                Application.Current.Dispatcher.Invoke(() => 
                {
                    var errorDialog = new ErrorDialog();
                    errorDialog.Initialize(this,
                        exception.Message,
                        exception.GetType().ToString(),
                        Properties.Resources.Ok,
                        string.Empty);

                    errorDialog.Show();
                });
            }
            finally
            {
                Application.Current.Shutdown();
            }
        };

        _backgroundWorker.RunWorkerAsync();
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        Closing -= MainWindow_Closing;
        _backgroundWorker?.CancelAsync();
        _backgroundWorker?.Dispose();
        _logBuilder.AppendLine();

        if (_isSuccess) return;
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "Update.log"), _logBuilder.ToString());
    }
}
