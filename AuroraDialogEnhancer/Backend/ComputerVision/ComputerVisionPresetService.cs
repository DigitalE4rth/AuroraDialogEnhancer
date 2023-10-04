using System.Linq;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.KeyBinding.Mappers;
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
        if (dialogOptionFinderProvider is null) return (false, $"{Properties.Localization.Resources.HookSettings_Error_Preset_Preset} {clientSize.Width}x{clientSize.Height} {Properties.Localization.Resources.HookSettings_Error_Preset_IsMissing}");

        _computerVisionService.Initialize(dialogOptionFinderProvider);
        _screenCaptureService.SetNameProvider(preset.GetScreenshotNameProvider());
        _cursorPositioningService.CursorPosition = dialogOptionFinderProvider.Data.InitialCursorPosition;

        var clickablePoints = preset.GetClickablePoints(clientSize);
        if (clickablePoints is not null)
        {
            var clickablePointMapper = new ClickablePrecisePointMapper();
            var mappedPoints = clickablePoints.Select(clickablePointMapper.Map).ToList();
            _keyHandlerService.InitializeClickablePoints(mappedPoints);
        }

        return (true, string.Empty);
    }
}
