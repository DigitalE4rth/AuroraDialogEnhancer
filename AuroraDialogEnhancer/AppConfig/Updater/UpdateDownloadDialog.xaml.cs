using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.AppConfig.WebClient;
using AuroraDialogEnhancer.Frontend.Forms.Utils;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public partial class UpdateDownloadDialog
{
    private readonly AdeWebClient _webClient;

    private UpdateInfo _updateInfo;
    private DateTime   _startedAt;
    private string     _tempFile;
    private bool       _isRunUpdateAsAdmin;

    public UpdateDownloadDialog()
    {
        Closing += UpdateDownloadDialog_Closing;

        _webClient  = new AdeWebClient();
        _updateInfo = new UpdateInfo();
        _tempFile   = string.Empty;

        InitializeComponent();
    }

    public void Initialize(UpdateInfo updateInfo, bool isRunUpdateAsAdmin)
    {
        _updateInfo         = updateInfo;
        _isRunUpdateAsAdmin = isRunUpdateAsAdmin;
        _tempFile           = Path.Combine(Global.Locations.AssemblyFolder, "Update_Download_Part.tmp");

        _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
        _webClient.DownloadFileCompleted   += OnDownloadFileCompleted;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        if (string.IsNullOrEmpty(_updateInfo.DownloadUri)) Close();

        var uri = new Uri(_updateInfo.DownloadUri);
        _startedAt = DateTime.Now - TimeSpan.FromSeconds(1);
        _webClient.DownloadFileAsync(uri, _tempFile);
    }

    private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        var timeSpan = DateTime.Now - _startedAt;
        var totalSeconds = (long)timeSpan.TotalSeconds;

        var bytesPerSecond = e.BytesReceived / totalSeconds;
        TextSpeed.Text = BytesToString(bytesPerSecond);
        TextSize.Text = $"{BytesToString(e.BytesReceived)} / {BytesToString(e.TotalBytesToReceive)}";
        ProgressBar.Value = e.ProgressPercentage;
    }

    private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs eventArgs)
    {
        if (eventArgs.Cancelled)
        {
            return;
        }

        try
        {
            if (eventArgs.Error != null)
            {
                throw eventArgs.Error;
            }

            ContentDisposition? contentDisposition = null;
            if (!string.IsNullOrWhiteSpace(_webClient.ResponseHeaders?["Content-Disposition"]))
            {
                try
                {
                    contentDisposition = new ContentDisposition(_webClient.ResponseHeaders!["Content-Disposition"]);
                }
                catch (FormatException)
                {
                    // Ignore content disposition header if it is wrongly formatted.
                    contentDisposition = null;
                }
            }

            var fileName = string.IsNullOrEmpty(contentDisposition?.FileName)
                ? Path.GetFileName(_webClient.ResponseUri!.LocalPath)
                : contentDisposition!.FileName;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new WebException(Properties.Localization.Resources.AutoUpdate_UnableToDetermineFileName);
            }

            var tempPath = Path.Combine(Global.Locations.AssemblyFolder, fileName);
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            File.Move(_tempFile, tempPath);

            var installerPath = Path.Combine(Path.GetDirectoryName(tempPath) ?? throw new InvalidOperationException(), "Updater.exe");
            File.WriteAllBytes(installerPath, Properties.InternalResources.Updater);

            var arguments = new Collection<string>
            {
                "--input",
                tempPath,
                "--output",
                Global.Locations.AssemblyFolder,
                "--current-exe",
                Global.Locations.AssemblyExe,
                "--updated-exe",
                Global.Locations.AssemblyExe
            };

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                arguments.Add("--args");
                arguments.Add(string.Join(" ", args.Skip(1).Select(arg => $"\"{arg}\"")));
            }

            var updaterProcessStartInfo = new ProcessStartInfo
            {
                FileName = installerPath,
                UseShellExecute = true,
                Arguments = new ArgumentProcessingService().BuildArguments(arguments)
            };

            if (_isRunUpdateAsAdmin)
            {
                updaterProcessStartInfo.Verb = "runas";
            }

            try
            {
                Process.Start(updaterProcessStartInfo);
            }
            catch (Win32Exception exception)
            {
                if (exception.NativeErrorCode != 1223)
                {
                    throw;
                }
            }

            DialogResult = true;
        }
        catch (Exception e)
        {
            new InfoDialogBuilder()
                .SetWindowTitle(Properties.Localization.Resources.EntityRepository_Error_Read)
                .SetMessage($"{e.Message}{Environment.NewLine}{e.InnerException?.Message}")
                .SetTypeError()
                .ShowDialog();
        }
        finally
        {
            Close();
        }
    }

    private string BytesToString(long byteCount)
    {
        string[] suf = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };
        if (byteCount == 0) return "0" + suf[0];

        var bytes = Math.Abs(byteCount);
        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return $"{(Math.Sign(byteCount) * num).ToString(CultureInfo.InvariantCulture)} {suf[place]}";
    }

    private void UpdateDownloadDialog_Closing(object sender, CancelEventArgs e)
    {
        if (_webClient is not { IsBusy: true }) return;

        _webClient.DownloadProgressChanged -= OnDownloadProgressChanged;
        _webClient.DownloadFileCompleted -= OnDownloadFileCompleted;
        _webClient.CancelAsync();
        _webClient.Dispose();
    }
}
