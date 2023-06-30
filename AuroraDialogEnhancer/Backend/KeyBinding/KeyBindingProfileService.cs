using System.IO;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.KeyBinding.Mappers;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyHandler;
using AuroraDialogEnhancerExtensions.Content;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Backend.KeyBinding;

public class KeyBindingProfileService
{
    private readonly HookedGameDataProvider      _hookedGameDataProvider;
    private readonly KeyBindingProfileMapper     _keyBindingProfileMapper;
    private readonly KeyBindingViewModelMapper   _viewModelMapper;
    private readonly KeyBindingProfileRepository _repository;
    private readonly ExtensionsProvider          _extensionsProvider;

    public KeyBindingProfileService(HookedGameDataProvider      hookedGameDataProvider,
                                    KeyBindingProfileMapper     keyBindingProfileMapper,
                                    KeyBindingViewModelMapper   viewModelMapper,
                                    KeyBindingProfileRepository repository,
                                    ExtensionsProvider          extensionsProvider)
    {
        _hookedGameDataProvider  = hookedGameDataProvider;
        _keyBindingProfileMapper = keyBindingProfileMapper;
        _viewModelMapper         = viewModelMapper;
        _repository              = repository;
        _extensionsProvider      = extensionsProvider;
    }

    #region Create/Update
    public void Create(ExtensionDto extension)
    {
        var profile = extension.GetKeyBindingProfile();
        Directory.CreateDirectory(Path.Combine(Global.Locations.ExtensionsFolder, extension.Name));
        Save(extension.Id, _keyBindingProfileMapper.Map(profile));
    }

    public void Save(string id, KeyBindingProfile profile)
    {
        var gameName = _extensionsProvider.ExtensionsDictionary[id].Name;
        var filePath = Path.Combine(Global.Locations.ExtensionsFolder, gameName, Global.Locations.KeyBindingProfilesFileName);

        _repository.Save(profile, filePath);
    }

    public void SaveDefault(string id)
    {
        var profile = _extensionsProvider.ExtensionsDictionary[id].GetKeyBindingProfile();
        SaveAndApplyIfHookIsActive(id, _keyBindingProfileMapper.Map(profile));
    }

    public void SavePristine(string id)
    {
        SaveAndApplyIfHookIsActive(id, new KeyBindingProfile());
    }

    public void SaveAndApplyIfHookIsActive(string id, KeyBindingProfileViewModel viewModel)
    {
        SaveAndApplyIfHookIsActive(id, _keyBindingProfileMapper.Map(viewModel));
    }

    public void SaveAndApplyIfHookIsActive(string id, KeyBindingProfile profile)
    {
        Save(id, profile);

        if (!_hookedGameDataProvider.IsExtenstionConfigPresent() ||
            _hookedGameDataProvider.Data!.ExtensionConfig!.Id != Properties.Settings.Default.UI_HookSettings_SelectedGameId)
        {
            return;
        }

        AppServices.ServiceProvider.GetRequiredService<KeyHandlerService>().ApplyKeyBinds();
    }
    #endregion



    #region Read

    public bool Exists(string gameName)
    {
        return File.Exists(Path.Combine(Global.Locations.ExtensionsFolder, gameName, Global.Locations.KeyBindingProfilesFileName));
    }

    public KeyBindingProfile Get(string id)
    {
        var gameName = _extensionsProvider.ExtensionsDictionary[id].Name;
        var filePath = Path.Combine(Global.Locations.ExtensionsFolder, gameName, Global.Locations.KeyBindingProfilesFileName);

        return _repository.Get(filePath);
    }

    public KeyBindingProfileViewModel GetViewModel(string id)
    {
        var profile = Get(id);
        return _viewModelMapper.Map(profile);
    }
    #endregion
}
