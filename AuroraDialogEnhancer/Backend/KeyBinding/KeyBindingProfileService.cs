using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.KeyBinding.Mappers;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;
using AuroraDialogEnhancer.Backend.KeyHandler;
using AuroraDialogEnhancerExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Backend.KeyBinding;

public class KeyBindingProfileService
{
    private readonly HookedGameDataProvider        _hookedGameDataProvider;
    private readonly KeyBindingViewModelBackMapper _viewModelBackMapper;
    private readonly KeyBindingViewModelMapper     _viewModelMapper;
    private readonly KeyBindingProfileRepository   _repository;
    private readonly ExtensionsProvider            _extensionsProvider;

    public KeyBindingProfileService(HookedGameDataProvider        hookedGameDataProvider,
                                    KeyBindingViewModelBackMapper viewModelBackMapper,
                                    KeyBindingViewModelMapper     viewModelMapper,
                                    KeyBindingProfileRepository   repository,
                                    ExtensionsProvider            extensionsProvider)
    {
        _hookedGameDataProvider = hookedGameDataProvider;
        _viewModelBackMapper    = viewModelBackMapper;
        _viewModelMapper        = viewModelMapper;
        _repository             = repository;
        _extensionsProvider     = extensionsProvider;
    }

    #region Create
    public void Create(ExtensionDto extension)
    {
        var extensionProfile = extension.GetKeyBindingProfileProvider().GetKeyBindingProfileDto();
        var mappedProfile = new KeyBindingExtensionMapper().Map(extensionProfile);

        Directory.CreateDirectory(Path.Combine(Global.Locations.ExtensionsFolder, extension.Name));
        Save(extension.Id, mappedProfile);
    }
    #endregion



    #region Update
    public void Save(string id, KeyBindingProfile profile)
    {
        var gameName = _extensionsProvider.ExtensionsDictionary[id].Name;
        var filePath = Path.Combine(Global.Locations.ExtensionsFolder, gameName, Global.Locations.KeyBindingProfilesFileName);

        _repository.Save(profile, filePath);
    }

    public void SaveDefault(string id)
    {
        var defaultProfile = GetDefault(id);
        SaveAndApplyIfHookIsActive(id, defaultProfile);
    }

    public void SaveEmpty(string id)
    {
        SaveAndApplyIfHookIsActive(id, new KeyBindingProfile());
    }

    public void SaveAndApplyIfHookIsActive(string id, KeyBindingProfileViewModel viewModel)
    {
        SaveAndApplyIfHookIsActive(id, _viewModelBackMapper.Map(viewModel));
    }

    public void SaveAndApplyIfHookIsActive(string id, KeyBindingProfile profiles)
    {
        Save(id, profiles);

        if (!_hookedGameDataProvider.IsExtenstionConfigPresent() ||
            !_hookedGameDataProvider.Data!.ExtensionConfig!.Id.Equals(Properties.Settings.Default.App_HookSettings_SelectedGameId, StringComparison.Ordinal))
        {
            return;
        }

        // ToDo: Messed up architecture щ(゜ロ゜щ)
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
        var extensionProfile = _extensionsProvider.ExtensionsDictionary[id].GetKeyBindingProfileProvider();

        return _viewModelMapper.Map((profile, extensionProfile.GetInteractionPointsVmDto()));
    }

    public KeyBindingProfile GetDefault(string id)
    {
        var extensionProfile = _extensionsProvider.ExtensionsDictionary[id].GetKeyBindingProfileProvider().GetKeyBindingProfileDto();
        return new KeyBindingExtensionMapper().Map(extensionProfile);
    }

    public List<List<GenericKey>> GetAllKeys(KeyBindingProfile keyBindingProfile)
    {
        var union = new List<List<GenericKey>>();
        union.AddRange(keyBindingProfile.PauseResume);
        union.AddRange(keyBindingProfile.Reload);
        union.AddRange(keyBindingProfile.Screenshot);
        union.AddRange(keyBindingProfile.HideCursor);
        union.AddRange(keyBindingProfile.Select);
        union.AddRange(keyBindingProfile.Previous);
        union.AddRange(keyBindingProfile.Next);
        keyBindingProfile.InteractionPoints.ForEach(point => union.AddRange(point.ActivationKeys));
        union.AddRange(keyBindingProfile.AutoSkipConfig.ActivationKeys);
        union.AddRange(keyBindingProfile.One);
        union.AddRange(keyBindingProfile.Two);
        union.AddRange(keyBindingProfile.Three);
        union.AddRange(keyBindingProfile.Four);
        union.AddRange(keyBindingProfile.Five);
        union.AddRange(keyBindingProfile.Six);
        union.AddRange(keyBindingProfile.Seven);
        union.AddRange(keyBindingProfile.Eight);
        union.AddRange(keyBindingProfile.Nine);
        union.AddRange(keyBindingProfile.Ten);
        return union;
    }

    public bool AreKeysAlreadyInUse(List<List<GenericKey>> keys, List<GenericKey> target)
    {
        return keys.FirstOrDefault(list => list.Count == target.Count && list.Except(target).ToList().Count == 0) is not null;
    }
    #endregion
}
