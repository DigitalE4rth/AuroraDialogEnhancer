using System.Collections.Generic;
using System.Drawing;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

public partial class DebugPage
{
    private readonly GameCvDialogOptionFinder _gameCvDialogOptionFinder;
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

    public DebugPage()
    {
        _gameCvDialogOptionFinder = new GameCvDialogOptionFinder();
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, System.Windows.RoutedEventArgs e)
    {
        // 720x576
        FindPoints("1920x1440");
        //_resolutionsTest.ForEach(FindPoints);
    }

    private void FindPoints(string clientSize)
    {
        using var image = new Bitmap($"D:\\Dev\\Projects\\E4rth_\\hoyo-dialog-enhancer-resources\\Raw Resolutions\\{clientSize}.png");
        _gameCvDialogOptionFinder.Initialize(image.Size);

        var result = _gameCvDialogOptionFinder.GetDialogOptions(image);
        System.Diagnostics.Debug.WriteLine($"Client size: {image.Size.Width}x{image.Size.Height}, Points count: {result.Count}");
        result.ForEach(point => System.Diagnostics.Debug.WriteLine(point));
    }
}
