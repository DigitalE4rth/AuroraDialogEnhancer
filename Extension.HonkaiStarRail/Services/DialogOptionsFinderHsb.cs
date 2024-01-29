using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;
using Extension.HonkaiStarRail.Templates;

namespace Extension.HonkaiStarRail.Services;

internal class DialogOptionsFinderHsb : DialogOptionsFinderBase<Hsba>
{
    public DialogOptionsFinderHsb(SearchTemplate searchTemplate) : base(searchTemplate)
    {
    }

    protected override int CountInRange(Bitmap image, ColorRange<Hsba> colorRange, int x, int y, int maxX, int maxY) =>
        BitmapUtils.CountInRange(image, colorRange, x, y, maxX, maxY);

    protected override bool IsWithinRange(Bitmap image, ColorRange<Hsba> colorRange, int x, int y) =>
        BitmapUtils.IsWithinRange(image, colorRange, x, y);

    protected override bool IsDarkerThenColor(Bitmap image, Hsba color, int x, int y) =>
        BitmapUtils.IsDarkerThenColor(image, color, x, y);
}
