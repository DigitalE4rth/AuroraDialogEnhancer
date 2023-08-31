using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Frontend.Forms.Utils;
using AuroraDialogEnhancer.Frontend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public class AutoUpdaterService
{
    private readonly UiService _uiService;
    private readonly bool      _isRunUpdateAsAdmin = true;

    private bool _isRunning;

    public AutoUpdaterService(UiService uiService)
    {
        _uiService = uiService;
    }

    public void Initialize()
    {
        CheckForUpdate();
    }

    private void CheckForUpdate()
    {
        if (Properties.Settings.Default.AutoUpdater_Frequency == TimeSpan.Zero ||
            Properties.Settings.Default.AutoUpdater_LastUpdateCheckTime + Properties.Settings.Default.AutoUpdater_Frequency > DateTime.Now) return;

        StartAuto();
    }

    public void StartManual() => Start(true, false);

    public void StartAuto() => Start(false, true);

    private void Start(bool isReportErrors, bool isSilentCheck)
    {
        if (_isRunning) return;
        _isRunning = true;

        try
        {
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
        }
        catch (NotSupportedException exception)
        {
            ShowError(exception, isReportErrors);
            _isRunning = false;
            return;
        }
        
        Task.Run(() => StartAsync(isReportErrors, isSilentCheck));
    }

    private void StartAsync(bool isReportErrors, bool isSilentCheck)
    {
        var (isUpdated, isSuccess) = (false, false);

        try
        {
            var updateInfo = GetUpdateInfo();
            (isUpdated, isSuccess) = StartUpdate(updateInfo, isReportErrors, isSilentCheck);
        }
        catch (Exception e)
        {
            ShowError(e, isReportErrors);
        }

        if (isUpdated)
        {
            Properties.Settings.Default.AutoUpdater_LastUpdateCheckTime = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        if (isSuccess)
        {
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown, DispatcherPriority.Send);
        }

        _isRunning = false;
    }

    private (bool, bool) StartUpdate(UpdateInfo updateInfo, bool isReportErrors, bool isSilentCheck)
    {
        if (updateInfo.IsUpdateAvailable)
        {
            return Application.Current.Dispatcher.Invoke(() => ShowUpdateDialog(updateInfo, isSilentCheck));
        }

        if (isReportErrors)
        {
            Application.Current.Dispatcher.Invoke(() =>
                new InfoDialogBuilder()
                    .SetWindowTitle(Properties.Localization.Resources.AutoUpdate_Unavailable_Title)
                    .SetMessage($"{Properties.Localization.Resources.AutoUpdate_Unavailable_Message}")
                    .SetTypeError()
                    .ShowDialog()
            );
        }

        return (false, false);
    }

    private UpdateInfo GetUpdateInfo()
    {
        var updateUri = new Uri(Properties.Settings.Default.AutoUpdate_UpdateInfoUri);

        UpdateInfo args;
        using (var client = new AdeWebClient())
        {
            var downloadString = client.DownloadString(updateUri);
            var xmlSerializer = new XmlSerializer(typeof(UpdateInfo));
            var xmlTextReader = new XmlTextReader(new StringReader(downloadString)) { XmlResolver = null };
            args = (UpdateInfo) xmlSerializer.Deserialize(xmlTextReader);
            
            args.DownloadUri  = TranslateUri(updateUri, args.DownloadUri);
            args.ChangelogUri = TranslateUri(updateUri, args.ChangelogUri);
        }

        if (string.IsNullOrEmpty(args.Version) || string.IsNullOrEmpty(args.DownloadUri))
        {
            throw new MissingFieldException();
        }
        
        args.InstalledVersion  = Assembly.GetExecutingAssembly().GetName().Version;
        args.IsUpdateAvailable = new Version(args.Version) > args.InstalledVersion;

        return args;
    }

    private void ShowError(Exception exception, bool isReportErrors)
    {
        if (!isReportErrors) return;

        if (exception is WebException)
        {
            Application.Current.Dispatcher.Invoke(() =>
                new InfoDialogBuilder()
                    .SetWindowTitle(Properties.Localization.Resources.AutoUpdate_Failed_Title)
                    .SetMessage($"{Properties.Localization.Resources.AutoUpdate_Failed_Message}{Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.InnerException?.Message}")
                    .SetTypeError()
                    .ShowDialog()
            );
            return;
        }

        Application.Current.Dispatcher.Invoke(() =>
            new InfoDialogBuilder()
                    .SetWindowTitle(Properties.Localization.Resources.AutoUpdate_Failed_Title)
                    .SetMessage($"{exception.Message}{Environment.NewLine}{exception.InnerException?.Message}")
                    .SetTypeError()
                    .ShowDialog()
            );
    }

    private (bool, bool) ShowUpdateDialog(UpdateInfo updateInfo, bool isSilentCheck)
    {
        if (isSilentCheck && !updateInfo.IsUpdateAvailable)
        {
            return (false, true);
        }

        var updateDialog = AppServices.ServiceProvider.GetRequiredService<UpdateDialog>();
        if (_uiService.IsMainWindowShown())
        {
            updateDialog.Owner = _uiService.GetMainWindow();
        }
        else
        {
            updateDialog.Title = $"{Global.AssemblyInfo.Name} | {Properties.Localization.Resources.AutoUpdate_WindowTitle}";
            updateDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        updateDialog.Initialize(updateInfo);

        if (updateDialog.ShowDialog() != true) return (false, true);

        if (ShowDownloadUpdateDialog(updateInfo) == false) return (false, true);

        return (true, true);
    }

    private bool ShowDownloadUpdateDialog(UpdateInfo updateInfo)
    {
        var downloadDialog = AppServices.ServiceProvider.GetRequiredService<UpdateDownloadDialog>();
        if (_uiService.IsMainWindowShown())
        {
            downloadDialog.Owner = _uiService.GetMainWindow();
        }
        else
        {
            downloadDialog.Title = $"{Global.AssemblyInfo.Name} | {Properties.Localization.Resources.AutoUpdate_WindowTitle_Download}";
            downloadDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        downloadDialog.Initialize(updateInfo, _isRunUpdateAsAdmin);
        return downloadDialog.ShowDialog() == true;
    }

    private string TranslateUri(Uri baseUri, string url)
    {
        if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Relative)) return url;

        var uri = new Uri(baseUri, url);

        if (uri.IsAbsoluteUri)
        {
            url = uri.AbsoluteUri;
        }

        return url;
    }
}
