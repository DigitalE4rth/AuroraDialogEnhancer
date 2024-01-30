using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Services;
using Extension.HonkaiStarRail.Templates;

namespace Extension.HonkaiStarRail.Services;

internal class DialogOptionsFinderRgb : DialogOptionsFinderBase<Rgba>
{
    public DialogOptionsFinderRgb(BitmapUtils bitmapUtils, SearchTemplate searchTemplate) : base(bitmapUtils, searchTemplate)
    {
    }

    protected override int CountInRange(Bitmap image, ColorRange<Rgba> colorRange, int x, int y, int maxX, int maxY) =>
        BitmapUtils.CountInRange(image, colorRange, x, y, maxX, maxY);

    protected override bool IsWithinRangeIcon(Bitmap image, ColorRange<Rgba> colorRange, int x, int y) =>
        BitmapUtils.IsWithinRange(image, colorRange, x, y);

    protected override bool IsWithinRangeText(Bitmap image, ColorRange<Rgba> colorRange, int x, int y) =>
        BitmapUtils.IsWithinRange(image, colorRange, x, y);

    protected override bool IsAreaContainsColor(Bitmap image, ColorRange<Rgba> colorRange, int x, int y, int maxX, int maxY) =>
        BitmapUtils.IsAreaContainsColor(image, colorRange, x, y, maxX, maxY);
}
