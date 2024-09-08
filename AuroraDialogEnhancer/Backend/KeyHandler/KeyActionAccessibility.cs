using System;
using System.Linq;
using AuroraDialogEnhancer.Backend.ComputerVision;
using AuroraDialogEnhancer.Backend.CursorPositioning;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using AuroraDialogEnhancer.Backend.KeyHandler.Scripts;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class KeyActionAccessibility : IDisposable
{
    private readonly FocusHookService         _focusHookService;
    private readonly ComputerVisionService    _computerVisionService;
    private readonly CursorPositioningService _cursorPositioningService;
    private readonly CursorVisibilityProvider _cursorVisibilityProvider;
    private readonly KeyActionUtility         _keyActionUtility;
    private readonly ScriptAutoSkipUtilities  _scriptAutoSkipUtilities;

    private bool _isLocked;
    private readonly object _lock = new();

    public KeyActionAccessibility(FocusHookService         focusHookService,
                                  ComputerVisionService    computerVisionService,
                                  CursorPositioningService cursorPositioningService,
                                  CursorVisibilityProvider cursorVisibilityProvider,
                                  KeyActionUtility         keyActionUtility,
                                  ScriptAutoSkipUtilities  scriptAutoSkipUtilities)
    {
        _focusHookService         = focusHookService;
        _computerVisionService    = computerVisionService;
        _cursorPositioningService = cursorPositioningService;
        _cursorVisibilityProvider = cursorVisibilityProvider;
        _keyActionUtility         = keyActionUtility;
        _scriptAutoSkipUtilities  = scriptAutoSkipUtilities;
    }

    /// <summary>
    /// Locks action execution thread.
    /// </summary>
    /// <returns><see langword="True"/> - if successfully locked. <see langword="False"/> - if another action is currently processing.</returns>
    private bool Lock()
    {
        lock (_lock)
        {
            if (_isLocked) return false;
            _isLocked = true;
            return true;
        }
    }

    /// <summary>
    /// Unlocks action execution thread.
    /// </summary>
    private void Unlock()
    {
        _isLocked = false;
    }

    /// <summary>
    /// Locks the thread and checks <see cref="IsAccessible">Accessibility</see>.
    /// </summary>
    /// <returns><see langword="True"/> - if thread is locked and further code execution is accessible; <see langword="Frue"/> - otherwise (thread will be unlocked automatically).</returns>
    private bool LockAndCheckAccessibilityAndUnlockIfNot()
    {
        if (!Lock()) return false;
        if (IsAccessible()) return true;
        _isLocked = false;
        return false;
    }

    /// <summary>
    /// Determines whether the application window is focused, whether the mouse cursor is displayed and whether it is inside the application window.
    /// </summary>
    /// <returns>
    /// <see langword="True"/> if further code execution is accessible; <see langword="False"/> otherwise.
    /// </returns>
    private bool IsAccessible()
    {
        return _focusHookService.IsFocused &&
               _cursorVisibilityProvider.IsVisible() &&
               _cursorPositioningService.IsCursorInsideClient();
    }

    /// <summary>
    /// Determines whether the code execution is <see cref="IsAccessible">Accessible</see> and whether <see cref="AreDialogOptionsPresent">Dialog options are present</see> on the screen.
    /// </summary>
    /// <returns>
    /// <see langword="True"/> if further code execution is processable; <see langword="False"/> otherwise.
    /// </returns>
    private bool IsProcessable()
    {
        return LockAndCheckAccessibilityAndUnlockIfNot() && CheckWhetherDialogOptionsArePresentAndUnlockIfNot();
    }

    /// <summary>
    /// Determines whether the key action can be performed, by checking whether Scripts are currently running, and whether code execution <see cref="IsProcessable">Is Processable</see>.
    /// </summary>
    /// <returns>
    /// <see langword="True"/> if further code execution is processable; <see langword="False"/> otherwise.
    /// </returns>
    private bool IsExecutable()
    {
        return !_scriptAutoSkipUtilities.IsAutoSkip &&
               _keyActionUtility.KeyBindingProfile.AutoSkipConfig.SkipMode != ESkipMode.Replies &&
               IsProcessable();
    }

    /// <summary>
    /// Checks whether Dialog Options are present, scans for them and checks again if not, and finally unlocks the thread if not.
    /// </summary>
    /// <returns>
    /// <see langword="True"/> if Dialog Options are present; <see langword="False"/> otherwise.
    /// </returns>
    private bool CheckWhetherDialogOptionsArePresentAndUnlockIfNot()
    {
        if (_keyActionUtility.DialogOptions.Any()) return true;

        if (!_computerVisionService.IsDialogMode())
        {
            _isLocked = false;
            return false;
        }

        _keyActionUtility.DialogOptions = _computerVisionService.GetDialogOptions();
        if (_keyActionUtility.DialogOptions.Any()) return true;

        if (!_scriptAutoSkipUtilities.IsAutoSkip && _keyActionUtility.KeyBindingProfile.CursorBehaviour == ECursorBehaviour.Hide) _cursorPositioningService.Hide();

        _isLocked = false;
        return false;
    }

    /// <summary>
    /// Checks whether Dialog Options are present, scans for them and checks again if not.
    /// </summary>
    /// <returns>
    /// <see langword="True"/> if Dialog Options are present; <see langword="False"/> otherwise.
    /// </returns>
    public bool AreDialogOptionsPresent()
    {
        if (_keyActionUtility.DialogOptions.Any()) return true;

        if (_computerVisionService.IsDialogMode() == false) return false;

        _keyActionUtility.DialogOptions = _computerVisionService.GetDialogOptions();
        if (_keyActionUtility.DialogOptions.Any()) return true;

        if (!_scriptAutoSkipUtilities.IsAutoSkip && _keyActionUtility.KeyBindingProfile.CursorBehaviour == ECursorBehaviour.Hide) _cursorPositioningService.Hide();

        return false;
    }

    /// <summary>
    /// Checks whether the Cursor and Speaker Name are present.
    /// </summary>
    /// <returns>
    /// <see langword="True"/> if Cursor and Speaker Name are present; <see langword="False"/> otherwise.
    /// </returns>
    public bool AreCursorAndSpeakerNamePresent()
    {
        return _computerVisionService.IsDialogMode() && _cursorVisibilityProvider.IsVisible();
    }

    /// <summary>
    /// Runs an action with an <see cref="IsAccessible">Access</see> check.
    /// </summary>
    /// <param name="action">Provided action.</param>
    public void WithAccess(Action action)
    {
        if (!Lock()) return;
        if (IsAccessible()) action.Invoke();
        _isLocked = false;
    }

    /// <summary>
    /// Runs an action with an <see cref="IsProcessable">Processability</see> check.
    /// </summary>
    /// <param name="action">Provided action.</param>
    public void WithProcess(Action action)
    {
        if (!IsProcessable()) return;
        action.Invoke();
        _isLocked = false;
    }

    /// <summary>
    /// Runs an action with an <see cref="IsExecutable">Execution</see> check.
    /// </summary>
    /// <param name="action">The action.</param>
    public void WithExecution(Action action)
    {
        if (!IsExecutable()) return;
        action.Invoke();
        _isLocked = false;
    }

    public void Dispose()
    {
        _isLocked = false;
    }
}
