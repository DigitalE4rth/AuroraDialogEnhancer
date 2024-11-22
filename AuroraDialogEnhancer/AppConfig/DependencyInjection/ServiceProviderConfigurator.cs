using AuroraDialogEnhancer.AppConfig.Config;
using AuroraDialogEnhancer.AppConfig.Localization;
using AuroraDialogEnhancer.AppConfig.NotifyIcon;
using AuroraDialogEnhancer.AppConfig.Theme;
using AuroraDialogEnhancer.AppConfig.Updater;
using AuroraDialogEnhancer.Backend.ComputerVision;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.CursorPositioning;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Global;
using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using AuroraDialogEnhancer.Backend.Hooks.Window;
using AuroraDialogEnhancer.Backend.Hooks.WindowGi;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;
using AuroraDialogEnhancer.Backend.KeyBinding.Mappers;
using AuroraDialogEnhancer.Backend.KeyHandler;
using AuroraDialogEnhancer.Backend.KeyHandlerScripts;
using AuroraDialogEnhancer.Backend.PeripheralEmulators;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancer.Backend.SoundPlayback;
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
        serviceCollection.AddSingleton<AutoUpdaterService>();
        serviceCollection.AddTransient<PatchService>();
        #endregion

        #region BackEnd
        #region Computer Vision
        serviceCollection.AddSingleton<ComputerVisionService>();
        serviceCollection.AddTransient<ComputerVisionPresetService>();
        #endregion

        #region Core
        serviceCollection.AddSingleton<CoreService>();
        #endregion

        #region Extensions
        serviceCollection.AddSingleton<ExtensionConfigRepository>();
        serviceCollection.AddSingleton<ExtensionConfigService>();
        serviceCollection.AddSingleton<ExtensionsProvider>();
        serviceCollection.AddSingleton<ExtensionsLoader>();
        #endregion

        #region HookedGameInfo
        serviceCollection.AddSingleton<ProcessDataProvider>();
        #endregion

        #region Hooks
        serviceCollection.AddSingleton<KeyboardHookManagerRecordService>();
        serviceCollection.AddSingleton<KeyboardHookManagerService>();
        serviceCollection.AddSingleton<ModifierKeysProvider>();

        serviceCollection.AddSingleton<CursorVisibilityProvider>();
        serviceCollection.AddSingleton<MouseEmulationService>();
        serviceCollection.AddTransient<MouseHookManagerRecordService>();
        serviceCollection.AddSingleton<MouseHookManagerService>();

        serviceCollection.AddSingleton<FocusHookDefaultService>();
        serviceCollection.AddSingleton<ProcessInfoService>();
        serviceCollection.AddSingleton<FocusHookService>();
        serviceCollection.AddSingleton<WindowLocationHook>();
        serviceCollection.AddSingleton<MinimizationEndHook>();
        serviceCollection.AddSingleton<MinimizationEndObserver>();
        serviceCollection.AddSingleton<MinimizationHook>();
        
        serviceCollection.AddSingleton<FocusHookGiService>();
        serviceCollection.AddSingleton<FocusHookGi>();
        serviceCollection.AddSingleton<MinimizationHookGi>();
        serviceCollection.AddSingleton<KeyboardFocusHook>();
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

        serviceCollection.AddSingleton<KeyActionMediator>();

        serviceCollection.AddSingleton<ScriptAutoClick>();
        serviceCollection.AddSingleton<ScriptAutoSkip>();
        serviceCollection.AddSingleton<ScriptAutoSkipUtilities>();

        serviceCollection.AddSingleton<KeyActionAccessibility>();
        serviceCollection.AddSingleton<KeyActionControls>();
        serviceCollection.AddSingleton<KeyActionExecution>();
        serviceCollection.AddSingleton<KeyActionRegistrar>();
        serviceCollection.AddSingleton<KeyActionUtility>();
        serviceCollection.AddSingleton<KeyPauseActionProvider>();
        #endregion

        #region Peripheral Emulators
        serviceCollection.AddSingleton<KeyboardEmulationService>();
        serviceCollection.AddSingleton<MouseEmulationService>();
        #endregion

        #region Screen capture
        serviceCollection.AddSingleton<ScreenCaptureService>();
        #endregion

        #region SoundPlayback
        serviceCollection.AddTransient<SoundPlaybackService>();
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
        serviceCollection.AddTransient<AutoSkipEditorWindow>();
        
        serviceCollection.AddTransient<MainPage>();

        serviceCollection.AddTransient<MenuPage>();

        serviceCollection.AddTransient<SettingsPage>();

        serviceCollection.AddTransient<AboutPage>();
        serviceCollection.AddTransient<UpdateDialog>();
        serviceCollection.AddTransient<UpdateDownloadDialog>();

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
