using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Services;
using Extension.HonkaiStarRail.Templates;

namespace Extension.HonkaiStarRail.Services;

internal class DialogOptionsFinderHsbDebug : DialogOptionsFinderDebugBase<Hsba>
{
    public DialogOptionsFinderHsbDebug(BitmapUtils bitmapUtils, SearchTemplate searchTemplate) : base(bitmapUtils, searchTemplate)
    {
    }

    protected bool IsWithinRangeIgnoreHue(Bitmap image, ColorRange<Hsba> colorRange, int x, int y)
    {
        var pixel = image.GetPixel(x, y);
        var pixelSaturation = pixel.GetSaturation();
        var pixelBrightness = pixel.GetBrightness();

        return pixelSaturation >= colorRange.Low.Saturation && pixelSaturation <= colorRange.High.Saturation &&
               pixelBrightness >= colorRange.Low.Brightness && pixelBrightness <= colorRange.High.Brightness;
    }

    protected override int CountInRange(Bitmap image, ColorRange<Hsba> colorRange, int x, int y, int maxX, int maxY) =>
        BitmapUtils.CountInRange(image, colorRange, x, y, maxX, maxY);

    protected override bool IsWithinRangeIcon(Bitmap image, ColorRange<Hsba> colorRange, int x, int y) =>
        IsWithinRangeIgnoreHue(image, colorRange, x, y);

    protected override bool IsWithinRangeText(Bitmap image, ColorRange<Hsba> colorRange, int x, int y) =>
        BitmapUtils.IsWithinRange(image, colorRange, x, y);

    protected override bool IsAreaContainsColor(Bitmap image, ColorRange<Hsba> colorRange, int x, int y, int maxX, int maxY) =>
        BitmapUtils.IsAreaContainsColor(image, colorRange, x, y, maxX, maxY);
}
