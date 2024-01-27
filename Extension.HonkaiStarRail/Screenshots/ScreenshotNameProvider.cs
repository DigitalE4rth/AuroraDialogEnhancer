using System;
using AuroraDialogEnhancerExtensions.Screenshots;

namespace Extension.HonkaiStarRail.Screenshots;

public class ScreenshotNameProvider : IScreenshotNameProvider
{
    public string GetName()
    {
        return $"StarRail_Image_{DateTimeOffset.Now.ToUnixTimeSeconds()}";
    }
}
