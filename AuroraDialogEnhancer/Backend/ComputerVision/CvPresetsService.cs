using System;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancerExtensions.Content;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class CvPresetsService
{
    private readonly CvPresetMapper  _cvPresetMapper;
    private readonly ExtensionsProvider _extensionsProvider;

    public CvPresetsService(ExtensionsProvider extensionsProvider)
    {
        _cvPresetMapper  = new CvPresetMapper();
        _extensionsProvider = extensionsProvider;
    }

    public (bool, string) SetPresets(HookedGameData hookedGameData)
    {
        var presetInfo = _extensionsProvider.ExtensionsDictionary[hookedGameData.ExtensionConfig!.Id];
        var clientSize = hookedGameData.GameWindowInfo!.ClientRectangle.Size;
        
        if (!presetInfo!.Presets.TryGetValue(clientSize, out var presetType))
        {
            return (false, Properties.Localization.Resources.HookSettings_Error_Preset_Preset
                           + " "
                           + $"{clientSize.Width}x{clientSize.Height}"
                           + " "
                           + Properties.Localization.Resources.HookSettings_Error_Preset_IsMissing);
        }

        var preset = (CvPresetDto)Activator.CreateInstance(presetType);
        hookedGameData.CvPreset = _cvPresetMapper.Map((CvPresetDto) Activator.CreateInstance(presetType));

        if (hookedGameData.CvPreset.DialogOptionTemplate is null)
        {
            return (false, Properties.Localization.Resources.HookSettings_Error_Preset_TemplateImage
                           + " "
                           + $"{clientSize.Width}x{clientSize.Height}"
                           + " "
                           + Properties.Localization.Resources.HookSettings_Error_Preset_IsMissing);
        }

        return (true, string.Empty);
    }
}
