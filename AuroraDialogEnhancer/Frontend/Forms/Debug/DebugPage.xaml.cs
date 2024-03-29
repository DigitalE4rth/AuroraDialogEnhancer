﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.AppConfig.Updater;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.PeripheralEmulators;
using AuroraDialogEnhancer.Backend.ScriptHandlers;
using AuroraDialogEnhancerExtensions.Dimensions;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

public partial class DebugPage
{
    private readonly AutoUpdaterService _updaterService;

    private readonly ScriptHandlerService _scriptHandlerService;
    //private readonly GameCvDialogOptionFinder _gameCvDialogOptionFinder;
    private readonly List<string> _resolutionsTest = new List<string>()
    {
        "640x480",
        "720x480",
        "720x576",
        "800x600",
        "1024x768",
        "1152x864",
        "1176x664",
        "1280x720",

        "1280x768",
        "1280x800",
        "1280x960",
        "1280x1024",
        "1280x1440",
        "1360x768",
        "1366x768",
        "1440x900",

        "1440x1080",
        "1600x900",
        "1600x1024",
        "1600x1200",
        "1680x1050",
        "1768x992",
        "1920x1080",
        "1920x1200",
        "1920x1440",
        "2560x1440",
    };

    public DebugPage(AutoUpdaterService updaterService, ScriptHandlerService scriptHandlerService)
    {
        //_updaterService = updaterService;
        //_scriptHandlerService = scriptHandlerService;

        //_gameCvDialogOptionFinder = new GameCvDialogOptionFinder();
        InitializeComponent();

        //_scriptHandlerService.AutoClickScript.Register(new List<GenericKey>{new KeyboardKey(0xA2), new KeyboardKey(65)});
        var r = GetHsba(new Bitmap("Debug/t.png"))
            .OrderBy(hsba => hsba.Brightness)
            .ToList();
        var t = r.Count;
    }

    private void ButtonBase_OnClick(object sender, System.Windows.RoutedEventArgs e)
    {
        Task.Delay(2000).Wait();
        _scriptHandlerService.AutoClickScript.DoAction();
        //_updaterService.CheckForUpdateManual();
        //_resolutionsTest.ForEach(path => Count($"D:\\Dev\\Projects\\E4rth_\\hoyo-dialog-enhancer-resources\\Raw Resolutions\\{path}.png"));
        //Count();
    }

    public List<Hsba> GetHsba(Bitmap image)
    {
        var r = new List<Hsba>();
        for (var i = 0; i < image.Height; i++)
        {
            for (var j = 0; j < image.Width; j++)
            {
                var pixel = image.GetPixel(j, i);
                r.Add(new Hsba((int) pixel.GetHue(), pixel.GetSaturation(), pixel.GetBrightness()));
            }
        }

        return r;
    }

    private void Count(string path)
    {
        var extensionProvider = AppServices.ServiceProvider.GetService<ExtensionsProvider>();
        var presetInfo = extensionProvider!.ExtensionsDictionary["GI"];
        var preset = presetInfo.GetPreset();
        using var image = new Bitmap(path);
        //using var image = new Bitmap("C:\\Games\\Genshin Impact\\Genshin Impact game\\ScreenShot\\20230821201400.png");
        //using var image = new Bitmap("D:\\Dev\\Projects\\E4rth_\\hoyo-dialog-enhancer-resources\\Raw Resolutions\\1440x1080.png");
        var finder = preset.GetDialogOptionFinderProvider(image.Size);
        using var croppedImage = GetArea(image, finder.Data.DialogDetectionConfig.DialogOptionsArea);
        var dialogOptions = finder.DialogOptionsFinder.GetDialogOptions(croppedImage);
        //var result = dialogOptions.Count;
        DrawRectangles(image, dialogOptions);
        image.Save($"D:\\Dev\\Projects\\E4rth_\\hoyo-dialog-enhancer-resources\\Raw Resolutions\\test\\{image.Width}x{image.Height}.png");
    }

    private void FindPoints(string clientSize)
    {
        /*using var image = new Bitmap($"D:\\Dev\\Projects\\E4rth_\\hoyo-dialog-enhancer-resources\\Raw Resolutions\\1\\{clientSize}.png");
        _gameCvDialogOptionFinder.Initialize(image.Size);

        var result = _gameCvDialogOptionFinder.GetDialogOptions(image);
        System.Diagnostics.Debug.WriteLine($"Client size: {image.Size.Width}x{image.Size.Height}, Points count: {result.Count}");
        result.ForEach(point => System.Diagnostics.Debug.WriteLine(point));*/
    }

    public void DrawRectangles(Image bitmap, List<Rectangle> rectangles)
    {
        using var graphics = Graphics.FromImage(bitmap);
        using var pen = new Pen(Color.LimeGreen, 1);
        rectangles.ForEach(r => graphics.DrawRectangle(pen, r));
    }

    private Bitmap GetArea(Bitmap image, Rectangle rectangle)
    {
        var croppedImage = new Bitmap(rectangle.Width, rectangle.Height);
        using var graphics = Graphics.FromImage(croppedImage);
        graphics.DrawImage(image, rectangle with { X = 0, Y = 0 }, rectangle, GraphicsUnit.Pixel);
        return croppedImage;
    }
}
