﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AuroraDialogEnhancer.Backend.ComputerVision;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using AuroraDialogEnhancer.Backend.Hooks.Window;
using AuroraDialogEnhancer.Backend.KeyHandler;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancer.Backend.Utils;

namespace AuroraDialogEnhancer.Backend.Core;

public class CoreService : IDisposable
{
    private readonly ComputerVisionPresetService _computerVisionPresetService;
    private readonly ExtensionConfigService      _extensionConfigService;
    private readonly FocusHookServiceFactory     _focusHookServiceFactory;
    private readonly KeyHandlerService           _keyHandlerService;
    private readonly MinimizationEndObserver     _minimizationEndObserver;
    private readonly MinimizationHookService     _minimizationHookService;
    private readonly ProcessDataProvider         _processDataProvider;
    private readonly ProcessInfoService          _processInfoService;
    private readonly ScreenCaptureService        _screenCaptureService;
    private readonly WindowLocationHookService   _windowLocationHookService;
    private          IGameFocusService           _gameFocusService;


    private readonly object _cancellingLock;
    private readonly object _processingLock;

    private Task? _autoDetectionTask;

    private bool _isAutoDetectionRunning;
    private bool _isCancellationRunning;

    private CancellationTokenSource? _cancellationTokenSource;

    public CoreService(ComputerVisionPresetService computerVisionPresetService,
                       ExtensionConfigService      extensionConfigService,
                       FocusHookServiceFactory     focusHookServiceFactory,
                       KeyHandlerService           keyHandlerService,
                       MinimizationEndObserver     minimizationEndObserver,
                       MinimizationHookService     minimizationHookService, 
                       ProcessDataProvider         processDataProvider,
                       ProcessInfoService          processInfoService,
                       ScreenCaptureService        screenCaptureService,
                       WindowLocationHookService   windowLocationHookService)
    {
        _computerVisionPresetService = computerVisionPresetService;
        _extensionConfigService      = extensionConfigService;
        _focusHookServiceFactory     = focusHookServiceFactory;
        _keyHandlerService           = keyHandlerService;
        _minimizationEndObserver     = minimizationEndObserver;
        _minimizationHookService     = minimizationHookService;
        _processDataProvider         = processDataProvider;
        _processInfoService          = processInfoService;
        _screenCaptureService        = screenCaptureService;
        _windowLocationHookService   = windowLocationHookService;
        _gameFocusService            = _focusHookServiceFactory.Get();
        
        _cancellingLock = new object();
        _processingLock = new object();
    }

    #region Controls
    public async Task RestartAutoDetection(string gameId, bool isRestart = false)
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

        var isStartAutoDetection = await CancelAndDetermineIfNeedToStart(gameId, isRestart);
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
        _processDataProvider.Id = gameId;

        if (!IsProcessInfoSpecified(extensionConfig))
        {
            _isAutoDetectionRunning = false;
            return;
        }

        _autoDetectionTask = StartAutoDetection(extensionConfig, isRestart);
        await _autoDetectionTask;
    }

    private bool IsResumePause()
    {
        if (_processDataProvider.HookState is not EHookState.Paused) return false;

        _keyHandlerService.OnPauseSwitch();
        return true;
    }
    
    private async Task<bool> CancelAndDetermineIfNeedToStart(string? gameId, bool restart)
    {
        if (_processDataProvider.Id is null) return true;
        var processingGameId = string.Copy(_processDataProvider.Id);
        var shouldRestart = (_processDataProvider.HookState is EHookState.Error or EHookState.Warning) || restart;

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

    private async Task StartAutoDetection(ExtensionConfig extensionConfig, bool isRestart)
    {
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();

            SetStateSearch();

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();
            
            await _processInfoService.AutoDetectProcessAsync(extensionConfig, _cancellationTokenSource!);
            
            _gameFocusService = _focusHookServiceFactory.Get(_processDataProvider.Id);
            _keyHandlerService.AttachFocusHook(_gameFocusService);
            _gameFocusService.SetWinEventHook();

            _processDataProvider.Data!.GameProcess!.Exited += ProcessOnExited;
            if (_processDataProvider.Data.GameProcess.HasExited)
            {
                if (HandleProcessExited()) return;

                SetStateNone();
                return;
            }

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            if (_processDataProvider.Data!.GameWindowInfo!.IsMinimized())
            {
                SetStateHooked();
            }

            if (!await _minimizationEndObserver.AwaitMinimizationEndAsync(_cancellationTokenSource!.Token)) return;

            _minimizationHookService.SetWinEventHook();
            _windowLocationHookService.SetWinEventHook();

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            var (isSuccess, message) = _computerVisionPresetService.SetPreset(_processDataProvider.Data);
            if (!isSuccess)
            {
                Dispose();
                SetStateError(message);
                return;
            }

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            _screenCaptureService.SetScreenshotsFolder(extensionConfig);
            _keyHandlerService.ApplyKeyBinds();
            _gameFocusService.SendFocusedEvent();
            if (isRestart) _keyHandlerService.HideCursorOnReload();
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
            _processDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_ProcessName_Game);
            return false;
        }

        switch (extensionConfig.HookLaunchType)
        {
            case EHookLaunchType.Game when string.IsNullOrEmpty(extensionConfig.GameLocation):
                _processDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_Location_Game_Empty);
                return false;
            case EHookLaunchType.Game when !File.Exists(extensionConfig.GameLocation):
                _processDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_Location_Game_NotExists);
                return false;
            case EHookLaunchType.Game:
                return true;
            case EHookLaunchType.Launcher when string.IsNullOrEmpty(extensionConfig.LauncherProcessName):
                _processDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_ProcessName_Launcher);
                return false;
            case EHookLaunchType.Launcher when string.IsNullOrEmpty(extensionConfig.LauncherLocation):
                _processDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_Location_Launcher_Empty);
                return false;
            case EHookLaunchType.Launcher when !File.Exists(extensionConfig.LauncherLocation):
                _processDataProvider.SetStateAndNotify(EHookState.Error, Properties.Localization.Resources.HookSettings_Error_Location_Launcher_NotExists);
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
            if (_processDataProvider.IsGameProcessAlive())
            {
                _processDataProvider.Data!.GameProcess!.Exited -= ProcessOnExited;
            }

            _processDataProvider.SetStateAndNotify(EHookState.Canceled);
            var isExitWithTheGame = _processDataProvider.IsExtenstionConfigPresent() &&
                                    _processDataProvider.Data!.ExtensionConfig!.IsExitWithTheGame;

            Dispose();
            _processDataProvider.SetStateAndNotify(EHookState.None);

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
            new FolderProcessStartService().Open(_extensionConfigService.GetScreenshotsLocation(id!));
        }

        _screenCaptureService.CapturedGames.Clear();
    }
    #endregion

    #region States
    private void SetStateSearch()
    {
        _processDataProvider.SetStateAndNotify(EHookState.Search);
    }

    private void SetStateNone()
    {
        _processDataProvider.Id = null;
        _processDataProvider.SetStateAndNotify(EHookState.None);
    }

    private void SetStateCancelling()
    {
        _processDataProvider.SetStateAndNotify(EHookState.Canceled);
    }

    private void SetStateHooked()
    {
        _processDataProvider.SetStateAndNotify(EHookState.Hooked);
    }

    private void SetStateError(string message)
    {
        _processDataProvider.SetStateAndNotify(EHookState.Error, message);
    }
    #endregion

    #region Cleanup
    private void DisposeAutoDetectionTask()
    {
        _minimizationHookService.UnhookWinEvent();
        _minimizationEndObserver.UnhookWinEvent();
        _windowLocationHookService.UnhookWinEvent();
        _gameFocusService.UnhookWinEvent();

        _keyHandlerService.Dispose();
        _processDataProvider.Dispose();
        _screenCaptureService.Dispose();

        if (_processDataProvider.IsGameProcessAlive())
        {
            _processDataProvider.Data!.GameProcess!.Exited -= ProcessOnExited;
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
