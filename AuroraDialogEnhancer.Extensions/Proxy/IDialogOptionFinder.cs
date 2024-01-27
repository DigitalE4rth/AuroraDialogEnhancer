using System.Collections.Generic;
using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public interface IDialogOptionFinder
{
    public bool IsDialogMode(params Bitmap[] image);

    public List<Rectangle> GetDialogOptions(Bitmap image);
}
