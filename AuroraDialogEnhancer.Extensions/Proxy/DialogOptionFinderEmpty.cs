using System.Collections.Generic;
using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

internal class DialogOptionFinderEmpty : IDialogOptionFinder
{
    public bool IsDialogMode(Bitmap image) => false;

    public List<Rectangle> GetDialogOptions(Bitmap image) => new(0);
}
