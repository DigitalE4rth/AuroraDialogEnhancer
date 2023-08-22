﻿using AuroraDialogEnhancer.AppConfig.Config;
using AuroraDialogEnhancer.AppConfig.Localization;
using AuroraDialogEnhancer.AppConfig.NotifyIcon;
using AuroraDialogEnhancer.AppConfig.Theme;
using AuroraDialogEnhancer.Backend.ComputerVision;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;
using AuroraDialogEnhancer.Backend.KeyBinding.Mappers;
using AuroraDialogEnhancer.Backend.KeyHandler;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancer.Backend.Utils;
using AuroraDialogEnhancer.Frontend.Forms;
using AuroraDialogEnhancer.Frontend.Forms.About;
using AuroraDialogEnhancer.Frontend.Forms.Appearance;
using AuroraDialogEnhancer.Frontend.Forms.Debug;
using AuroraDialogEnhancer.Frontend.Forms.HookControl;
using AuroraDialogEnhancer.Frontend.Forms.KeyBinding;
using AuroraDialogEnhancer.Frontend.Forms.Main;
using AuroraDialogEnhancer.Frontend.Forms.Menu;
using AuroraDialogEnhancer.Frontend.Forms.PresetsEditor;
using AuroraDialogEnhancer.Frontend.Forms.Settings;
using AuroraDialogEnhancer.Frontend.Forms.Utils;
using AuroraDialogEnhancer.Frontend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.AppConfig.DependencyInjection;

internal class ServiceProviderConfigurator
{
    public ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        #region AppConfig
        serviceCollection.AddSingleton<Config.AppConfig>();
        serviceCollection.AddSingleton<SingleInstanceService>();
        serviceCollection.AddTransient<CultureProvider>();
        serviceCollection.AddTransient<CultureService>();
        serviceCollection.AddSingleton<NotifyIconService>();
        serviceCollection.AddTransient<NotifyMenuWindow>();
        serviceCollection.AddTransient<ColorThemeService>();
        #endregion

        #region BackEnd
        #region Core
        serviceCollection.AddSingleton<CoreService>();
        #endregion

        #region ExtensionConfig
        serviceCollection.AddSingleton<ExtensionConfigRepository>();
        serviceCollection.AddSingleton<ExtensionConfigService>();
        #endregion

        #region HookedGameInfo
        serviceCollection.AddSingleton<HookedGameDataProvider>();
        #endregion

        #region Hooks
        serviceCollection.AddSingleton<KeyboardHookManagerRecordService>();
        serviceCollection.AddSingleton<KeyboardHookManagerService>();
        serviceCollection.AddSingleton<ModifierKeysProvider>();

        serviceCollection.AddSingleton<CursorVisibilityStateProvider>();
        serviceCollection.AddSingleton<MouseEmulationService>();
        serviceCollection.AddTransient<MouseHookManagerRecordService>();
        serviceCollection.AddSingleton<MouseHookManagerService>();

        serviceCollection.AddSingleton<ProcessInfoService>();
        serviceCollection.AddSingleton<ProcessStartService>();
        serviceCollection.AddSingleton<WindowHookService>();
        #endregion

        #region KeyBinding
        serviceCollection.AddSingleton<KeyboardKeyInterpreterService>();
        serviceCollection.AddSingleton<MouseKeyInterpreterService>();
        serviceCollection.AddSingleton<KeyInterpreterService>();

        serviceCollection.AddSingleton<KeyBindingProfileRepository>();
        serviceCollection.AddSingleton<KeyBindingProfileService>();
        serviceCollection.AddSingleton<KeyBindingViewModelBackMapper>();
        serviceCollection.AddSingleton<KeyBindingViewModelMapper>();
        #endregion

        #region KeyHandler
        serviceCollection.AddSingleton<CursorPositioningService>();
        serviceCollection.AddSingleton<KeyHandlerService>();
        #endregion

        #region OpenCv
        serviceCollection.AddSingleton<ComputerVisionService>();
        serviceCollection.AddTransient<ComputerVisionPresetService>();
        #endregion

        #region Presets
        serviceCollection.AddSingleton<ExtensionsProvider>();
        serviceCollection.AddSingleton<ExtensionsLoader>();
        #endregion

        #region Screen capture
        serviceCollection.AddSingleton<ScreenCaptureService>();
        #endregion

        #region Utils
        serviceCollection.AddTransient<BlobToBitmapImageConverter>();
        #endregion
        #endregion



        #region FrontEnd
        #region Forms
        serviceCollection.AddTransient<AppearancePage>();
        
        serviceCollection.AddTransient<HookControlPage>();

        serviceCollection.AddTransient<KeyBindingPage>();
        serviceCollection.AddTransient<TriggerEditorWindow>();

        serviceCollection.AddTransient<MainPage>();

        serviceCollection.AddTransient<MenuPage>();

        serviceCollection.AddTransient<SettingsPage>();

        serviceCollection.AddTransient<AboutPage>();

        serviceCollection.AddTransient<MissingExtensionsPage>();

        serviceCollection.AddTransient<DebugPage>();

        serviceCollection.AddTransient<PresetsEditorPage>();

        serviceCollection.AddTransient<MainWindow>();
        serviceCollection.AddSingleton<UiService>();
        #endregion

        #region Services
        serviceCollection.AddTransient<KeyCapsService>();
        #endregion
        #endregion

        return serviceCollection.BuildServiceProvider();
    }
}
