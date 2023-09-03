using System;

namespace AuroraDialogEnhancerExtensions.Screenshots;

internal class ScreenshotNameProviderDefault : IScreenshotNameProvider
{
    public string GetName() => DateTime.Now.ToString("yyyMMddHHmmss");
}
