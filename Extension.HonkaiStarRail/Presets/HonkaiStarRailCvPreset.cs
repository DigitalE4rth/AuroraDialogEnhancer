using AuroraDialogEnhancerExtensions.Content;
using AuroraDialogEnhancerExtensions.Utilities;

namespace Extension.HonkaiStarRail.Presets;

public class HonkaiStarRailCvPreset : CvPresetDto
{
    public override ColorRange? SpeakerNameColorRange { get; protected set; } = new(new Rgba(255, 230, 170, 0), new Rgba(255, 255, 210, 10));
}
