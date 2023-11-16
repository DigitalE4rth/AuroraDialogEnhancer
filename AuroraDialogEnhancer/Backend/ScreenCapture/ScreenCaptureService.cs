using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.SoundPlayback;
using AuroraDialogEnhancerExtensions.Screenshots;
using Clipboard = System.Windows.Forms.Clipboard;

namespace AuroraDialogEnhancer.Backend.ScreenCapture;

public class ScreenCaptureService : IDisposable
{
    private readonly HookedGameDataProvider _hookedGameDataProvider;
    private readonly SoundPlaybackService _soundPlaybackService;
    
    private IScreenshotNameProvider? _screenshotNameProvider;
    private readonly Queue<(string, Bitmap)> _imagesToSaveQueue;
    private readonly bool _isCopyToBuffer;
    private readonly object _lock;
    private string? _screenshotsFolder;
    public HashSet<string> CapturedGames { get; }

    public ScreenCaptureService(HookedGameDataProvider hookedGameDataProvider, 
                                SoundPlaybackService   soundPlaybackService)
    {
        _hookedGameDataProvider = hookedGameDataProvider;
        _soundPlaybackService   = soundPlaybackService;

        _imagesToSaveQueue = new Queue<(string, Bitmap)>();
        _lock = new object();
        _isCopyToBuffer = false;
        CapturedGames = new HashSet<string>();
    }

    public void SetNameProvider(IScreenshotNameProvider screenshotNameProvider)
    {
        _screenshotNameProvider = screenshotNameProvider;
    }

    public void SetScreenshotsFolder(ExtensionConfig? extensionConfig)
    {
        if (extensionConfig is null)
        {
            _screenshotsFolder = null;
            return;
        }

        var path = string.IsNullOrEmpty(extensionConfig.ScreenshotsLocation) 
            ? Path.Combine(Global.Locations.ExtensionsFolder, extensionConfig.Name, Global.Locations.ScreenshotsFolderName)
            : extensionConfig.ScreenshotsLocation;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        _screenshotsFolder = path;
    }

    public void CaptureAndSave()
    {
        if (!CanBeCaptured()) return;

        var frame = CaptureClient();
        if (frame is null) return;

        Task.Run(PlayCaptureSound).ConfigureAwait(false);
        _imagesToSaveQueue.Enqueue((_screenshotNameProvider!.GetName(), frame));

        SaveImage();
        CapturedGames.Add(_hookedGameDataProvider.Data!.ExtensionConfig!.Id);
    }

    private void PlayCaptureSound()
    {
        if (!Properties.Settings.Default.App_IsScreenshotSound) return;
        _soundPlaybackService.PlaySound(Properties.InternalResources.screenshot);
    }

    private void SaveImage()
    {
        lock (_lock)
        {
            var (imageName, bitmap) = _imagesToSaveQueue.Dequeue();
            var path = GetFilePath(imageName);

            bitmap.Save(path, ImageFormat.Png);

            if (_isCopyToBuffer)
            {
                var bitmapCopy = new Bitmap(bitmap);

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Clipboard.SetImage(new Bitmap(bitmapCopy));
                    bitmapCopy.Dispose();
                    bitmapCopy = null;
                });
            }

            bitmap.Dispose();
        }
    }

    private string GetFilePath(string imageName)
    {
        var path = Path.Combine(_screenshotsFolder!, $"{imageName}.png");
        if (!File.Exists(path)) return path;

        var fileNumber = 2;
        while (File.Exists(path))
        {
            path = Path.Combine(_screenshotsFolder!, $"{imageName} ({fileNumber}).png");
            fileNumber++;
        }

        return path;
    }

    private Bitmap? CaptureClient()
    {
        if (!CanBeCaptured()) return null;

        return Capture(new Rectangle(
            _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X,
            _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y,
            _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Width,
            _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangle.Height));
    }

    public Bitmap CaptureRelative(Rectangle rectangle)
    {
        if (!CanBeCaptured()) return new Bitmap(0,0);

        var relativeRectangle = new Rectangle(
            _hookedGameDataProvider.Data!.GameWindowInfo!.ClientRectangleRelativePosition.X + rectangle.X,
            _hookedGameDataProvider.Data.GameWindowInfo.ClientRectangleRelativePosition.Y + rectangle.Y,
            rectangle.Width,
            rectangle.Height);

        return Capture(relativeRectangle);
    }

    private Bitmap Capture(Rectangle rectangle)
    {
        var bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppRgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(rectangle.X, rectangle.Y, 0, 0, bitmap.Size);

        return bitmap;
    }

    private bool CanBeCaptured()
    {
        return _hookedGameDataProvider.IsGameProcessAlive() &&
               _hookedGameDataProvider.Data!.GameWindowInfo is not null &&
              !_hookedGameDataProvider.Data.GameWindowInfo.IsMinimized();
    }

    public void Dispose()
    {
        SetScreenshotsFolder(null);
        _screenshotNameProvider = null;
    }
}
