using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Frontend.Forms.Menu;
using AuroraDialogEnhancer.Frontend.Forms.Utils;
using AuroraDialogEnhancer.Frontend.Services;
using AuroraDialogEnhancerExtensions.Content;

namespace AuroraDialogEnhancer.Backend.Extensions;

public class ExtensionsLoader
{
    private readonly ExtensionConfigService   _extensionConfigService;
    private readonly KeyBindingProfileService _keyBindingProfileService;
    private readonly ExtensionsProvider       _extensionsProvider;
    private readonly UiService                _uiService;

    public ExtensionsLoader(ExtensionConfigService    extensionConfigService,
                            KeyBindingProfileService keyBindingProfileService, 
                            ExtensionsProvider       extensionsProvider, 
                            UiService                uiService)
    {
        _extensionConfigService   = extensionConfigService;
        _keyBindingProfileService = keyBindingProfileService;
        _extensionsProvider       = extensionsProvider;
        _uiService                = uiService;
    }

    public void Initialize()
    {
        Directory.CreateDirectory(Global.Locations.ExtensionsFolder);

        var extensionPaths = new List<string>();
        foreach (var extensionsDirectory in Directory.GetDirectories(Global.Locations.ExtensionsFolder))
        {
            var dllFiles = Directory.GetFiles(extensionsDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            extensionPaths.AddRange(dllFiles);
        }

        var extensionsEnumerable = from   extension in extensionPaths
                                   select Assembly.LoadFrom(extension) into extensionAssembly
                                   select extensionAssembly.GetTypes().FirstOrDefault(eType => typeof(ExtensionDto).IsAssignableFrom(eType)) into extensionType
                                   where  extensionType is not null
                                   select (ExtensionDto) Activator.CreateInstance(extensionType);

        _extensionsProvider.Initialize(extensionsEnumerable);

        if (!_extensionsProvider.ExtensionsDictionary.Any())
        {
            _uiService.SetNewRoute(EPageType.HookSettings, typeof(MissingExtensionsPage));
            _uiService.SetNewRoute(EPageType.KeyBinding, typeof(MissingExtensionsPage));
            return;
        }

        CreateConfigIfNotExists();

        if (_extensionsProvider.ExtensionsDictionary.ContainsKey(Properties.Settings.Default.UI_HookSettings_SelectedGameId)) return;
        Properties.Settings.Default.UI_HookSettings_SelectedGameId = _extensionsProvider.ExtensionsDictionary.Keys.First();
        Properties.Settings.Default.Save();
    }

    public void CreateConfigIfNotExists()
    {
        if (!Directory.Exists(Global.Locations.ExtensionsFolder))
        {
            Directory.CreateDirectory(Global.Locations.ExtensionsFolder);
        }

        foreach (var extension in _extensionsProvider.ExtensionsDictionary.Values)
        {
            if (!_extensionConfigService.Exists(extension.Name))
            {
                _extensionConfigService.Create(extension);
            }

            if (!_keyBindingProfileService.Exists(extension.Name))
            {
                _keyBindingProfileService.Create(extension);
            }

            var screenshotsFolder = Path.Combine(Global.Locations.ExtensionsFolder, extension.Name, Global.Locations.ScreenshotsFolderName);
            if (!Directory.Exists(screenshotsFolder))
            {
                Directory.CreateDirectory(screenshotsFolder);
            }
        }
    }
}
