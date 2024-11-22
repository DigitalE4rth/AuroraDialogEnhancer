using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AuroraDialogEnhancer.Backend.ComputerVision;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Global;
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
    private readonly GlobalFocusService          _globalFocusHookService;
    private readonly KeyActionControls           _keyActionControls;
    private readonly KeyActionExecution          _keyActionExecution;
    private readonly KeyActionMediator           _keyActionMediator;
    private readonly MinimizationEndObserver     _minimizationEndObserver;
    private readonly MinimizationHook            _minimizationHook;
    private readonly ProcessDataProvider         _processDataProvider;
    private readonly ProcessInfoService          _processInfoService;
    private readonly ScreenCaptureService        _screenCaptureService;
    private readonly WindowLocationHook          _windowLocationHook;
    
    private Task?                    _autoDetectionTask;
    private SemaphoreSlim?           _autoDetectionSemaphore;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly object          _processExitLock;
    private readonly object          _processingLock;
    
    public bool IsProcessing { get; private set; }
    public event EventHandler<bool>? OnProcessing;

    public CoreService(ComputerVisionPresetService computerVisionPresetService,
                       ExtensionConfigService      extensionConfigService,
                       GlobalFocusService          globalFocusHookService,
                       KeyActionControls           keyActionControls,
                       KeyActionExecution          keyActionExecution,
                       KeyActionMediator           keyActionMediator,
                       MinimizationEndObserver     minimizationEndObserver,
                       MinimizationHook            minimizationHook, 
                       ProcessDataProvider         processDataProvider,
                       ProcessInfoService          processInfoService,
                       ScreenCaptureService        screenCaptureService,
                       WindowLocationHook          windowLocationHook)
    {
        _computerVisionPresetService = computerVisionPresetService;
        _extensionConfigService      = extensionConfigService;
        _globalFocusHookService      = globalFocusHookService;
        _keyActionControls           = keyActionControls;
        _keyActionExecution          = keyActionExecution;
        _keyActionMediator           = keyActionMediator;
        _minimizationEndObserver     = minimizationEndObserver;
        _minimizationHook            = minimizationHook;
        _processDataProvider         = processDataProvider;
        _processInfoService          = processInfoService;
        _screenCaptureService        = screenCaptureService;
        _windowLocationHook          = windowLocationHook;

        _processExitLock = new object();
        _processingLock  = new object();
    }

    #region Control
    public void Run(string id, EStartMode mode = EStartMode.Default) => Task.Run(() => WithLock(() => Resolve(id, mode)));
    
    private void WithLock(Action action)
    {
        lock (_processingLock)
        {
            IsProcessing = true;
            OnProcessing?.Invoke(this, true);
            action.Invoke();
            IsProcessing = false;
            OnProcessing?.Invoke(this, false);
        }
    }

    private void Resolve(string id, EStartMode mode)
    {
        if (_processDataProvider.Id is not null &&
            !id.Equals(_processDataProvider.Id))
        {
            Switch(id, mode);
            return;
        }

        switch (_processDataProvider.HookState)
        {
            case EHookState.Paused:
                _keyActionControls.ResumeFromPauseIfPaused();
                return;
            case EHookState.Error or EHookState.Warning:
                Stop();
                Start(id, mode);
                return;
            case EHookState.Search or EHookState.Hooked:
                if (mode is EStartMode.StartOnly) return;
                Stop();
                if (mode is not EStartMode.Restart) return;
                Start(id, mode);
                return;
            case EHookState.None:
                Start(id, mode);
                return;
            case EHookState.Switch:
                return;
            default:
                Stop();
                return;
        }
    }

    private void Start(string id, EStartMode mode)
    {
        var extensionConfig = _extensionConfigService.Get(id);
        _processDataProvider.Id = id;
        _cancellationTokenSource = new CancellationTokenSource();
        _autoDetectionSemaphore = new SemaphoreSlim(0);
        _globalFocusHookService.SetWinEventHook();
        
        SetStateSearch();
        _autoDetectionTask = StartAutoDetection(extensionConfig, mode);
    }
    
    private void Stop()
    {
        SetStateCancel();
        ReleaseResources();
        SetStateNone();
    }

    private void Switch(string id, EStartMode mode)
    {
        SetStateSwitch();
        ReleaseResources();
        Start(id, mode);
    }
    
    private async Task StartAutoDetection(ExtensionConfig extensionConfig, EStartMode mode)
    {
        try
        {
            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();
            if (!ValidateAndNotifyOnError(extensionConfig)) return;

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();
            await _processInfoService.StartAndDetectProcessAsync(extensionConfig, _cancellationTokenSource!);

            _processDataProvider.Data!.GameProcess!.Exited += ProcessOnExited;
            if (_processDataProvider.Data.GameProcess.HasExited)
            {
                if (HandleProcessExited()) return;
                SetStateNone();
                return;
            }

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();
            _keyActionControls.InitializeFocusHook();

            if (_processDataProvider.Data!.GameWindowInfo!.IsMinimized())
            {
                SetStateHooked();
            }

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();
            if (!await _minimizationEndObserver.AwaitMinimizationEndAsync(_cancellationTokenSource!.Token)) return;
            _minimizationHook.SetWinEventHook();
            _windowLocationHook.SetWinEventHook();

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
            _keyActionControls.ApplyKeyBinds();
            _globalFocusHookService.SendFocusedEvent();
            Debug.WriteLine("AuroraDialogEnhancer: Started");
            if (mode is EStartMode.Restart) _keyActionExecution.HideCursorOnReload();
            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();

            SetStateHooked();
        }
        catch
        {
            _autoDetectionSemaphore?.Release();
            Dispose();
        }
        
        _autoDetectionSemaphore?.Release();
    }
    #endregion

    #region Validators
    private bool ValidateAndNotifyOnError(ExtensionConfig extensionConfig)
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
        lock (_processExitLock)
        {
            if (_processDataProvider.IsGameProcessAlive())
            {
                _processDataProvider.Data!.GameProcess!.Exited -= ProcessOnExited;
            }
            
            List<(string id, string processName)> idToProcessNamePair = _extensionConfigService.GetIdToProcessNameList(_processDataProvider.Id!);
            if (idToProcessNamePair.Any())
            {
                var processes = Process.GetProcesses();
                var nextExtensionToHookId = idToProcessNamePair.FirstOrDefault(pair => processes.Any(process => pair.processName!.Equals(process.ProcessName)));
                if (nextExtensionToHookId != (null, null))
                {
                    Run(nextExtensionToHookId.id, EStartMode.StartOnly);
                    return true;
                }
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
    
    private void SetStateSwitch()
    {
        _processDataProvider.SetStateAndNotify(EHookState.Switch);
    }

    private void SetStateNone()
    {
        _processDataProvider.Id = null;
        _processDataProvider.SetStateAndNotify(EHookState.None);
    }

    private void SetStateCancel()
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

    private void ReleaseResources()
    {
        _minimizationHook.UnhookWinEvent();
        _minimizationEndObserver.UnhookWinEvent();
        _windowLocationHook.UnhookWinEvent();
        
        _keyActionMediator.Dispose();
        _processDataProvider.Dispose();
        _screenCaptureService.Dispose();

        if (_processDataProvider.IsGameProcessAlive())
            _processDataProvider.Data!.GameProcess!.Exited -= ProcessOnExited;

        _cancellationTokenSource?.Cancel();

        if (_autoDetectionTask is { IsCompleted: false })
            _autoDetectionSemaphore?.Wait();
        
        _autoDetectionSemaphore?.Dispose();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _autoDetectionSemaphore = null;
        _autoDetectionTask = null;
    }
    
    public void Dispose()
    {
        ReleaseResources();
        _globalFocusHookService.UnhookWinEvent();
    }
    #endregion
}
