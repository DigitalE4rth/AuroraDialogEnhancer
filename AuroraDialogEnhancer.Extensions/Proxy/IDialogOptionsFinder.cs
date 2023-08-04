using System.Collections.Generic;
using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public interface IDialogOptionsFinder
{
    public bool IsDialogMode(Bitmap image);

    public List<Rectangle> GetDialogOptions(Bitmap image);
}
