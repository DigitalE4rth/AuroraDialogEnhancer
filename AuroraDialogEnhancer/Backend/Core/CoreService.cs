using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.ComputerVision;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using AuroraDialogEnhancer.Backend.KeyHandler;
using AuroraDialogEnhancer.Backend.ScreenCapture;

namespace AuroraDialogEnhancer.Backend.Core;

public class CoreService : IDisposable
{
    private readonly ComputerVisionPresetService _computerVisionPresetService;
    private readonly ExtensionConfigService      _extensionConfigService;
    private readonly HookedGameDataProvider      _hookedGameDataProvider;
    private readonly KeyHandlerService           _keyHandlerService;
    private readonly ProcessInfoService          _processInfoService;
    private readonly ScreenCaptureService        _screenCaptureService;
    private readonly WindowHookService           _windowHookService;
    
    private readonly object _cancellingLock;
    private readonly object _processingLock;

    private Task? _autoDetectionTask;

    private bool _isAutoDetectionRunning;
    private bool _isCancellationRunning;

    private CancellationTokenSource? _cancellationTokenSource;

    public CoreService(ComputerVisionPresetService computerVisionPresetService,
                       ExtensionConfigService      extensionConfigService,
                       HookedGameDataProvider      hookedGameDataProvider,
                       KeyHandlerService           keyHandlerService,
                       ProcessInfoService          processInfoService,
                       ScreenCaptureService        screenCaptureService,
                       WindowHookService           windowHookService)
    {
        _computerVisionPresetService = computerVisionPresetService;
        _extensionConfigService      = extensionConfigService;
        _hookedGameDataProvider      = hookedGameDataProvider;
        _keyHandlerService           = keyHandlerService;
        _processInfoService          = processInfoService;
        _screenCaptureService        = screenCaptureService;
        _windowHookService           = windowHookService;

        _cancellingLock = new object();
        _processingLock = new object();
    }

    #region Controls
    public async Task RestartAutoDetection(string gameId, bool restart = false)
    {
        lock (_cancellingLock)
        {
            if (_isCancellationRunning) return;
            _isCancellationRunning = true;
        }

        if (IsResumePause())
        {
            _isCancellationRunning = false;
            return;
        }

        var isStartAutoDetection = await CancelAndDetermineIfNeedToStart(gameId, restart);
        if (!isStartAutoDetection)
        {
            _isAutoDetectionRunning = false;
            _isCancellationRunning  = false;
            return;
        }

        lock (_processingLock)
        {
            if (_isAutoDetectionRunning) return;
            _isAutoDetectionRunning = true;
            _isCancellationRunning  = false;
        }

        var extensionConfig = _extensionConfigService.Get(gameId);
        _hookedGameDataProvider.Id = gameId;

        if (!IsProcessInfoSpecified(extensionConfig))
        {
            _isAutoDetectionRunning = false;
            return;
        }

        _autoDetectionTask = StartAutoDetection(extensionConfig);
        await _autoDetectionTask;
    }

    private bool IsResumePause()
    {
        if (_hookedGameDataProvider.HookState is not EHookState.Paused) return false;

        _keyHandlerService.OnPauseSwitch();
        return true;
    }

    private async Task<bool> CancelAndDetermineIfNeedToStart(string? gameId, bool restart)
    {
        if (_hookedGameDataProvider.Id is null) return true;
        var processingGameId = string.Copy(_hookedGameDataProvider.Id);
        var shouldRestart = (_hookedGameDataProvider.HookState is EHookState.Error or EHookState.Warning) || restart;

        SetStateCancelling();

        // Cancel running search.
        _cancellationTokenSource?.Cancel();
        Dispose();

        // Canceled. Waiting for cleanup.
        if (_autoDetectionTask is { IsCompleted: false })
        {
            await _autoDetectionTask;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        _autoDetectionTask = null;

        // Stop if current.
        if (processingGameId != gameId || shouldRestart) return true;

        SetStateNone();
        return false;
    }

    private async Task StartAutoDetection(ExtensionConfig extensionConfig)
    {
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();

            SetStateSearch();

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            _keyHandlerService.AttachFocusHook();
            _windowHookService.InitializeFocusHook();

            await _processInfoService.AutoDetectProcessAsync(extensionConfig, _cancellationTokenSource!);

            _hookedGameDataProvider.Data!.GameProcess!.Exited += ProcessOnExited;
            if (_hookedGameDataProvider.Data.GameProcess.HasExited)
            {
                if (HandleProcessExited()) return;

                SetStateNone();
                return;
            }

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            if (_hookedGameDataProvider.Data!.GameWindowInfo!.IsMinimized)
            {
                SetStateHooked();
            }

            if (!await _windowHookService.AwaitMinimizationEndAsync(_cancellationTokenSource!.Token)) return;

            _windowHookService.InitializeWindowLocationHook();

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            var (isSuccess, message) = _computerVisionPresetService.SetPreset(_hookedGameDataProvider.Data);
            if (!isSuccess)
            {
                Dispose();
                SetStateError(message);
                return;
            }

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            _screenCaptureService.SetScreenshotsFolder(extensionConfig);
            _keyHandlerService.ApplyKeyBinds();
            _windowHookService.SendFocusedEvent();

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            SetStateHooked();
        }
        catch
        {
            DisposeAutoDetectionTask();
        }
    }
    #endregion

    #region Validators
    private bool IsProcessInfoSpecified(ExtensionConfig extensionConfig)
    {
        if (string.IsNullOrEmpty(extensionConfig.GameProcessName))
        {
            _hookedGameDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_ProcessName_Game);
            return false;
        }

        switch (extensionConfig.HookLaunchType)
        {
            case EHookLaunchType.Game when string.IsNullOrEmpty(extensionConfig.GameLocation):
                _hookedGameDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_Location_Game_Empty);
                return false;
            case EHookLaunchType.Game when !File.Exists(extensionConfig.GameLocation):
                _hookedGameDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_Location_Game_NotExists);
                return false;
            case EHookLaunchType.Game:
                return true;
            case EHookLaunchType.Launcher when string.IsNullOrEmpty(extensionConfig.LauncherProcessName):
                _hookedGameDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_ProcessName_Launcher);
                return false;
            case EHookLaunchType.Launcher when string.IsNullOrEmpty(extensionConfig.LauncherLocation):
                _hookedGameDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_Location_Launcher_Empty);
                return false;
            case EHookLaunchType.Launcher when !File.Exists(extensionConfig.LauncherLocation):
                _hookedGameDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_Location_Launcher_NotExists);
                return false;
            case EHookLaunchType.Launcher:
                return true;
            case EHookLaunchType.Nothing:
                return true;
            default:
                return true;
        }
    }
    #endregion

    #region Process Exit
    private void ProcessOnExited(object sender, EventArgs e) => HandleProcessExited();

    private bool HandleProcessExited()
    {
        lock (_processingLock)
        {
            if (_hookedGameDataProvider.IsGameProcessAlive())
            {
                _hookedGameDataProvider.Data!.GameProcess!.Exited -= ProcessOnExited;
            }

            _hookedGameDataProvider.SetStateAndNotify(EHookState.Canceled);
            var isExitWithTheGame = _hookedGameDataProvider.IsExtenstionConfigPresent() &&
                                    _hookedGameDataProvider.Data!.ExtensionConfig!.IsExitWithTheGame;

            Dispose();
            _hookedGameDataProvider.SetStateAndNotify(EHookState.None);

            OpenScreenshotFolders();

            if (!isExitWithTheGame) return false;

            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown, DispatcherPriority.Send);
            return true;
        }
    }

    private void OpenScreenshotFolders()
    {
        if (!Properties.Settings.Default.App_IsScreenshotsManager || !_screenCaptureService.CapturedGames.Any()) return;

        foreach (var id in _screenCaptureService.CapturedGames)
        {
            var screenshotsLocation = _extensionConfigService.GetScreenshotsLocation(id);
            Process.Start(new ProcessStartInfo(Global.StringConstants.ExplorerName, screenshotsLocation));
        }

        _screenCaptureService.CapturedGames.Clear();
    }
    #endregion

    #region States
    private void SetStateSearch()
    {
        _hookedGameDataProvider.SetStateAndNotify(EHookState.Search);
    }

    private void SetStateNone()
    {
        _hookedGameDataProvider.Id = null;
        _hookedGameDataProvider.SetStateAndNotify(EHookState.None);
    }

    private void SetStateCancelling()
    {
        _hookedGameDataProvider.SetStateAndNotify(EHookState.Canceled);
    }

    private void SetStateHooked()
    {
        _hookedGameDataProvider.Data!.GameWindowInfo!.SetClientSize();
        _hookedGameDataProvider.SetStateAndNotify(EHookState.Hooked);
    }

    private void SetStateError(string message)
    {
        _hookedGameDataProvider.SetStateAndNotify(EHookState.Error, message);
    }
    #endregion

    #region Cleanup
    private void DisposeAutoDetectionTask()
    {
        _windowHookService.Dispose();
        _keyHandlerService.Dispose();
        _hookedGameDataProvider.Dispose();
        _screenCaptureService.SetScreenshotsFolder(null);

        if (_hookedGameDataProvider.IsGameProcessAlive())
        {
            _hookedGameDataProvider.Data!.GameProcess!.Exited -= ProcessOnExited;
        }

        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _autoDetectionTask = null;
        _isAutoDetectionRunning = false;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();

        if (_autoDetectionTask?.IsCompleted == false)
        {
            _autoDetectionTask?.Wait();
        }

        DisposeAutoDetectionTask();
    }
    #endregion
}
