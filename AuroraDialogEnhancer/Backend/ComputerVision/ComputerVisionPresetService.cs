using System.Linq;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;
using AuroraDialogEnhancer.Backend.KeyHandler;
using AuroraDialogEnhancer.Backend.ScreenCapture;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class ComputerVisionPresetService
{
    private readonly CursorPositioningService _cursorPositioningService;
    private readonly ComputerVisionService    _computerVisionService;
    private readonly ExtensionsProvider       _extensionsProvider;
    private readonly KeyHandlerService        _keyHandlerService;
    private readonly ScreenCaptureService     _screenCaptureService;

    public ComputerVisionPresetService(CursorPositioningService cursorPositioningService,
                                       ComputerVisionService    computerVisionService,
                                       ExtensionsProvider       extensionsProvider,
                                       KeyHandlerService        keyHandlerService, 
                                       ScreenCaptureService     screenCaptureService)
    {
        _cursorPositioningService = cursorPositioningService;
        _computerVisionService    = computerVisionService;
        _extensionsProvider       = extensionsProvider;
        _keyHandlerService        = keyHandlerService;
        _screenCaptureService     = screenCaptureService;
    }

    public (bool, string) SetPreset(HookedGameData hookedGameData)
    {
        var presetInfo = _extensionsProvider.ExtensionsDictionary[hookedGameData.ExtensionConfig!.Id];
        var clientSize = hookedGameData.GameWindowInfo!.ClientRectangle.Size;

        var preset = presetInfo.GetPreset();

        var dialogOptionFinderProvider = preset.GetDialogOptionFinderProvider(clientSize);
        if (dialogOptionFinderProvider.Data.DialogDetectionConfig.DialogOptionsArea.IsEmpty) return (false, $"{Properties.Localization.Resources.HookSettings_Error_Preset_Preset} {clientSize.Width}x{clientSize.Height} {Properties.Localization.Resources.HookSettings_Error_Preset_IsMissing}");

        _computerVisionService.Initialize(dialogOptionFinderProvider);
        _screenCaptureService.SetNameProvider(preset.GetScreenshotNameProvider());
        _cursorPositioningService.InitialCursorData(dialogOptionFinderProvider.Data.CursorData);

        var interactionPoints = preset.GetInteractionPoints(clientSize);
        if (interactionPoints.Any())
        {
            var mapper = new InteractionPrecisePointMapper();
            var mappedPoints = interactionPoints.Select(mapper.Map).ToList();
            _keyHandlerService.InitializeInteractionPoints(mappedPoints);
        }

        return (true, string.Empty);
    }
}
