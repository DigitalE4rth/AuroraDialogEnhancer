using AuroraDialogEnhancerExtensions.Content;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Presets;

public class GenshinImpactCvPreset : CvPresetDto
{
    public override ColorRange? SpeakerNameColorRange { get; protected set; } = new(new Rgba(255, 230, 170, 0), new Rgba(255, 255, 210, 10));
}
