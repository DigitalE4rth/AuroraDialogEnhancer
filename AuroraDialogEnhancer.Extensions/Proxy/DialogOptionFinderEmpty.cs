using System.Collections.Generic;
using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class DialogOptionFinderEmpty : IDialogOptionFinder
{
    public bool IsDialogMode(Bitmap image) => false;

    public List<Rectangle> GetDialogOptions(Bitmap image) => new(0);
}
