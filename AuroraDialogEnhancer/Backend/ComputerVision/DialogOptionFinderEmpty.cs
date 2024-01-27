using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.Proxy;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class DialogOptionFinderEmpty : IDialogOptionFinder
{
    public bool IsDialogMode(params Bitmap[] image) => false;

    public List<Rectangle> GetDialogOptions(Bitmap image) => new(0);
}
