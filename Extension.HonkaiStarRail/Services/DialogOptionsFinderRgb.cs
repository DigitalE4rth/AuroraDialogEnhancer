using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;
using Extension.HonkaiStarRail.Templates;

namespace Extension.HonkaiStarRail.Services;

internal class DialogOptionsFinderRgb : DialogOptionsFinderBase<Rgba>
{
    public DialogOptionsFinderRgb(SearchTemplate searchTemplate) : base(searchTemplate)
    {
    }

    protected override int CountInRange(Bitmap image, ColorRange<Rgba> colorRange, int x, int y, int maxX, int maxY) =>
        BitmapUtils.CountInRange(image, colorRange, x, y, maxX, maxY);

    protected override bool IsWithinRange(Bitmap image, ColorRange<Rgba> colorRange, int x, int y) =>
        BitmapUtils.IsWithinRange(image, colorRange, x, y);

    protected override bool IsDarkerThenColor(Bitmap image, Rgba color, int x, int y) =>
        BitmapUtils.IsDarkerThenColor(image, color, x, y);
}
