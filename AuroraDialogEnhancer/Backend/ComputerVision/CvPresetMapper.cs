using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancerExtensions.Content;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class CvPresetMapper : IMapper<CvPresetDto, CvPreset>
{
    public CvPreset Map(CvPresetDto obj)
    {
        return new CvPreset
        {
            // General
            Resolution = obj.Resolution,

            // Search regions
            SpeakerNameSearchRegion = obj.SpeakerNameSearchRegion,
            SpeakerNameColorRange   = new ColorRangeScalar(obj.SpeakerNameColorRange!),

            // Dialog option
            DialogOptionSearchRegion          = obj.DialogOptionSearchRegion,
            Threshold                         = obj.Threshold,
            DialogOptionTemplate              = new DialogOptionTemplate(obj.GetDialogOptionTemplate(), obj.GetDialogOptionMask()),
            DialogOptionRegion                = obj.DialogOptionRegion,
            DialogOptionInitialCursorPosition = obj.DialogOptionInitialCursorPosition,
            DialogOptionGap                   = obj.DialogOptionGap,

            // Buttons
            AutoSkipLocation        = obj.AutoSkipLocation,
            HideUiLocation          = obj.HideUiLocation,
            FullScreenPopUpLocation = obj.FullScreenPopUpLocation,

            // Utils
            HiddenCursorLocation = obj.HiddenCursorLocation
        };
    }
}
