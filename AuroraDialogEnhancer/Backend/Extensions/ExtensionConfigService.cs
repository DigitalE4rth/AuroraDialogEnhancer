﻿using System;
using System.IO;
using System.Threading.Tasks;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancerExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Backend.Extensions;

public class ExtensionConfigService
{
    private readonly ExtensionConfigRepository _extensionConfigRepository;
    private readonly ExtensionsProvider        _extensionsProvider;
    private readonly ProcessDataProvider       _processDataProvider;
    private readonly ScreenCaptureService      _screenCaptureService;

    public ExtensionConfigService(ExtensionConfigRepository extensionConfigRepository,
                                  ProcessDataProvider       processDataProvider,
                                  ExtensionsProvider        extensionsProvider,
                                  ScreenCaptureService      screenCaptureService)
    {
        _extensionConfigRepository = extensionConfigRepository;
        _processDataProvider       = processDataProvider;
        _extensionsProvider        = extensionsProvider;
        _screenCaptureService      = screenCaptureService;
    }

    #region Create/Update
    public void Create(ExtensionDto extension)
    {
        Directory.CreateDirectory(Path.Combine(Global.Locations.ExtensionsFolder, extension.Name));
        Save(new ExtensionConfigMapper().Map(extension));
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

    #region Update
    public void SaveAndRestartHookIfNecessary(ExtensionConfig config)
    {
        _extensionConfigRepository.Save(config, Path.Combine(Global.Locations.ExtensionsFolder, config.Name, Global.Locations.ExtensionConfigFileName));

        if (_processDataProvider.Id is not null &&
            _processDataProvider.Id.Equals(config.Id, StringComparison.Ordinal) &&
            _processDataProvider.HookState is not EHookState.None)
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
        if (_processDataProvider.Data?.ExtensionConfig is null ||
            !_processDataProvider.Data.ExtensionConfig.Id.Equals(config.Id, StringComparison.Ordinal)) return;

        _screenCaptureService.SetScreenshotsFolder(config);
    }

    public ExtensionConfig? UpdateLocations(string id)
    {
        var extension = _extensionsProvider.ExtensionsDictionary[id];
        var filePath = Path.Combine(Global.Locations.ExtensionsFolder, extension.Name, Global.Locations.ExtensionConfigFileName);
        var config = _extensionConfigRepository.Get(filePath);
        var locationProvider = extension.GetLocationProvider();

        var isUpdated = false;
        if (!string.IsNullOrEmpty(locationProvider.GameLocation))
        {
            config.GameLocation = locationProvider.GameLocation;
            isUpdated = true;
        }

        if (!string.IsNullOrEmpty(locationProvider.LauncherLocation))
        {
            config.LauncherLocation = locationProvider.LauncherLocation;
            isUpdated = true;
        }

        if (!string.IsNullOrEmpty(locationProvider.ScreenshotsLocation))
        {
            config.ScreenshotsLocation = locationProvider.ScreenshotsLocation;
            isUpdated = true;
        }

        if (!isUpdated) return null;

        SaveAndRestartHookIfNecessary(config);
        return config;
    }
    #endregion
}
