using System;
using System.IO;
using System.Threading.Tasks;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancerExtensions;
using AuroraDialogEnhancerExtensions.Content;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Backend.Extensions;

public class ExtensionConfigService
{
    private readonly ExtensionConfigRepository _extensionConfigRepository;
    private readonly HookedGameDataProvider    _hookedGameDataProvider;
    private readonly ExtensionsProvider        _extensionsProvider;
    private readonly ScreenCaptureService      _screenCaptureService;

    public ExtensionConfigService(ExtensionConfigRepository extensionConfigRepository,
                                  HookedGameDataProvider    hookedGameDataProvider,
                                  ExtensionsProvider        extensionsProvider,
                                  ScreenCaptureService      screenCaptureService)
    {
        _extensionConfigRepository = extensionConfigRepository;
        _hookedGameDataProvider    = hookedGameDataProvider;
        _extensionsProvider        = extensionsProvider;
        _screenCaptureService      = screenCaptureService;
    }

    #region Create/Update
    public void Create(ExtensionDto extension)
    {
        Directory.CreateDirectory(Path.Combine(Global.Locations.ExtensionsFolder, extension.Name));
        Save(new ExtensionConfigMapper().Map(extension));
    }

    public void SaveAndRestartHookIfNecessary(ExtensionConfig config)
    {
        _extensionConfigRepository.Save(config, Path.Combine(Global.Locations.ExtensionsFolder, config.Name, Global.Locations.ExtensionConfigFileName));

        if (_hookedGameDataProvider.Id is not null &&
            _hookedGameDataProvider.Id.Equals(config.Id, StringComparison.Ordinal) &&
            _hookedGameDataProvider.HookState is not EHookState.None)
        {
            Task.Run(() => AppServices.ServiceProvider.GetRequiredService<CoreService>().RestartAutoDetection(config.Id, true)).ConfigureAwait(false);
        }
    }

    public void Save(ExtensionConfig config)
    {
        _extensionConfigRepository.Save(config, Path.Combine(Global.Locations.ExtensionsFolder, config.Name, Global.Locations.ExtensionConfigFileName));
    }

    public void SetScreenshotsFolderForActiveGameIfNecessary(ExtensionConfig config)
    {
        if (_hookedGameDataProvider.Data?.ExtensionConfig is null ||
            !_hookedGameDataProvider.Data.ExtensionConfig.Id.Equals(config.Id, StringComparison.Ordinal)) return;

        _screenCaptureService.SetScreenshotsFolder(config);
    }

    public void SaveDefault(string id)
    {
        SaveAndRestartHookIfNecessary(new ExtensionConfigMapper().Map(_extensionsProvider.ExtensionsDictionary[id]));
    }
    #endregion



    #region Read
    public ExtensionConfig Get(string id)
    {
        var extension = _extensionsProvider.ExtensionsDictionary[id];
        var filePath = Path.Combine(Global.Locations.ExtensionsFolder, extension.Name, Global.Locations.ExtensionConfigFileName);

        return _extensionConfigRepository.Get(filePath);
    }

    public ExtensionConfigViewModel GerViewModel(string id)
    {
        return new ExtensionConfigViewModel(Get(id));
    }

    public string GetScreenshotsLocation(string id)
    {
        var extensionConfig = Get(id);
        return string.IsNullOrEmpty(extensionConfig.ScreenshotsLocation) 
            ? Path.Combine(Global.Locations.ExtensionsFolder, extensionConfig.Name, Global.Locations.ScreenshotsFolderName)
            : extensionConfig.ScreenshotsLocation;
    }

    public bool Exists(string fileName)
    {
        return File.Exists(Path.Combine(Global.Locations.ExtensionsFolder, fileName, Global.Locations.ExtensionConfigFileName));
    }
    #endregion
}
