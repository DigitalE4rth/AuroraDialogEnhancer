using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    
    private readonly Dictionary<string, ExtensionConfig> _idToConfigDict      = new();
    private          Dictionary<string, string>          _processNameToIdDict = new();
    private          Dictionary<string, string>          _idToProcessNameDict = new();

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
    public void Initialize()
    {
        foreach (var pair in _extensionsProvider.ExtensionsDictionary)
        {
            if (string.IsNullOrEmpty(pair.Key)) continue;
            var filePath = Path.Combine(AppConstants.Locations.ExtensionsFolder, pair.Value.Name, AppConstants.Locations.ExtensionConfigFileName);
            var extension = _extensionConfigRepository.Get(filePath);
            _idToConfigDict.Add(pair.Key, extension);
            
            _idToProcessNameDict.Add(pair.Key, extension.GameProcessName);
            if (_processNameToIdDict.ContainsKey(extension.GameProcessName)) continue;
            _processNameToIdDict.Add(extension.GameProcessName, pair.Key);
        }
    }

    private void UpdateDictionariesInfo()
    {
        var newIdToProcessNameDict = new Dictionary<string, string>();
        var newProcessNameToIdDict = new Dictionary<string, string>();
        
        foreach (var pair in _idToConfigDict)
        {
            newIdToProcessNameDict.Add(pair.Key, pair.Value.GameProcessName);
            if (newProcessNameToIdDict.ContainsKey(pair.Value.GameProcessName)) continue;
            newProcessNameToIdDict.Add(pair.Value.GameProcessName, pair.Key);
        }
        
        _processNameToIdDict = newIdToProcessNameDict;
        _idToProcessNameDict = newProcessNameToIdDict;
    }
    
    public void Create(ExtensionDto extension)
    {
        Directory.CreateDirectory(Path.Combine(AppConstants.Locations.ExtensionsFolder, extension.Name));
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
        return _idToConfigDict[id];
    }

    public ExtensionConfigViewModel GerViewModel(string id)
    {
        return new ExtensionConfigViewModel(Get(id));
    }

    public List<(string, string)> GetIdToProcessNameList(string exceptId = "")
    {
        return _idToProcessNameDict
            .Where(pair => !pair.Key.Equals(exceptId))
            .Select(pair => (pair.Key, pair.Value))
            .ToList();
    }

    public string? GetIdByProcessName(string processName)
    {
        return _processNameToIdDict.TryGetValue(processName, out var id) ? id : null;
    }
    
    public string GetScreenshotsLocation(string id)
    {
        var extensionConfig = Get(id);
        return string.IsNullOrEmpty(extensionConfig.ScreenshotsLocation) 
            ? Path.Combine(AppConstants.Locations.ExtensionsFolder, extensionConfig.Name, AppConstants.Locations.ScreenshotsFolderName)
            : extensionConfig.ScreenshotsLocation;
    }

    public bool Exists(string fileName)
    {
        return File.Exists(Path.Combine(AppConstants.Locations.ExtensionsFolder, fileName, AppConstants.Locations.ExtensionConfigFileName));
    }
    #endregion

    #region Update
    public void SaveAndRestartHookIfNecessary(ExtensionConfig config)
    {
        _extensionConfigRepository.Save(config, Path.Combine(AppConstants.Locations.ExtensionsFolder, config.Name, AppConstants.Locations.ExtensionConfigFileName));

        if (_processDataProvider.Id is not null &&
            _processDataProvider.Id.Equals(config.Id, StringComparison.Ordinal) &&
            _processDataProvider.HookState is not EHookState.None)
        {
            AppServices.ServiceProvider.GetRequiredService<CoreService>().Run(config.Id, EStartMode.Restart);
        }
    }

    public void Save(ExtensionConfig config)
    {
        _extensionConfigRepository.Save(config, Path.Combine(AppConstants.Locations.ExtensionsFolder, config.Name, AppConstants.Locations.ExtensionConfigFileName));
        _idToConfigDict[config.Id] = config;
        UpdateDictionariesInfo();
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
        var filePath = Path.Combine(AppConstants.Locations.ExtensionsFolder, extension.Name, AppConstants.Locations.ExtensionConfigFileName);
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
