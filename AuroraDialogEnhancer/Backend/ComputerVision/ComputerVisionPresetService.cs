using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.KeyHandler;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class ComputerVisionPresetService
{
    private readonly CursorPositioningService _cursorPositioningService;
    private readonly ComputerVisionService    _computerVisionService;
    private readonly ExtensionsProvider       _extensionsProvider;
    private readonly KeyHandlerService        _keyHandlerService;

    public ComputerVisionPresetService(CursorPositioningService cursorPositioningService,
                                       ComputerVisionService    computerVisionService,
                                       ExtensionsProvider       extensionsProvider,
                                       KeyHandlerService        keyHandlerService)
    {
        _cursorPositioningService = cursorPositioningService;
        _computerVisionService    = computerVisionService;
        _extensionsProvider       = extensionsProvider;
        _keyHandlerService        = keyHandlerService;
    }

    public (bool, string) SetPreset(HookedGameData hookedGameData)
    {
        var presetInfo = _extensionsProvider.ExtensionsDictionary[hookedGameData.ExtensionConfig!.Id];
        var clientSize = hookedGameData.GameWindowInfo!.ClientRectangle.Size;

        var preset = presetInfo.GetPreset();

        var doFinderProvider = preset.GetDialogOptionFinderProvider(clientSize);
        if (doFinderProvider is null) return (false, $"{Properties.Localization.Resources.HookSettings_Error_Preset_Preset} {clientSize.Width}x{clientSize.Height} {Properties.Localization.Resources.HookSettings_Error_Preset_IsMissing}");

        _computerVisionService.Initialize(doFinderProvider);
        _cursorPositioningService.CursorPosition = doFinderProvider.Data.InitialCursorPosition;

        var clickablePoints = preset.GetClickablePoints(clientSize);
        if (clickablePoints is not null)
        {
            _keyHandlerService.InitializeClickablePoints(clickablePoints);
        }
        
        return (true, string.Empty);
    }
}
