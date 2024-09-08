using System.Diagnostics;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.KeyHandler.Scripts;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class KeyPauseActionProvider
{
    private          KeyActionRegistrar? _keyActionRegistrar;
    private readonly KeyActionUtility    _keyActionUtility;
    private readonly ProcessDataProvider _processDataProvider;
    private readonly ScriptAutoSkip      _scriptAutoSkip;

    public KeyPauseActionProvider(KeyActionUtility    keyActionUtility,
                                  ProcessDataProvider processDataProvider,
                                  ScriptAutoSkip      scriptAutoSkip)
    {
        _keyActionUtility    = keyActionUtility;
        _processDataProvider = processDataProvider;
        _scriptAutoSkip      = scriptAutoSkip;
    }

    public void Initialize(KeyActionRegistrar keyActionRegistrar)
    {
        _keyActionRegistrar = keyActionRegistrar;
    }

    public void OnPauseSwitch()
    {
        if (_keyActionUtility.IsHookPause)
        {
            Debug.WriteLine("R");
            _keyActionRegistrar!.UnRegister(_keyActionUtility.KeyBindingProfile.PauseResume);
            _keyActionRegistrar.RegisterKeyBinds();

            _processDataProvider.SetStateAndNotify(EHookState.Hooked);
            _keyActionUtility.IsHookPause = false;
            return;
        }

        Debug.WriteLine("P");

        _scriptAutoSkip.Stop();

        _keyActionRegistrar!.UnRegisterAll();
        _keyActionRegistrar.Register(_keyActionUtility.KeyBindingProfile.PauseResume, OnPauseSwitch);

        _processDataProvider.SetStateAndNotify(EHookState.Paused);
        _keyActionUtility.IsHookPause = true;
    }
}
