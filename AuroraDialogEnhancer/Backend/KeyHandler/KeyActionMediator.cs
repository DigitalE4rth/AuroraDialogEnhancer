using System;
using AuroraDialogEnhancer.Backend.KeyHandlerScripts;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class KeyActionMediator : IDisposable
{
    private readonly KeyActionAccessibility _keyActionAccessibility;
    private readonly KeyActionControls      _keyActionControls;
    private readonly KeyActionRegistrar     _keyActionRegistrar;
    private readonly KeyActionUtility       _keyActionUtility;
    private readonly ScriptAutoClick        _scriptAutoClick;
    private readonly ScriptAutoSkip         _scriptAutoSkip;

    public KeyActionMediator(KeyActionAccessibility keyActionAccessibility,
                             KeyActionControls      keyActionControls,
                             KeyActionRegistrar     keyActionRegistrar,
                             KeyActionUtility       keyActionUtility,
                             ScriptAutoClick        scriptAutoClick,
                             ScriptAutoSkip         scriptAutoSkip)
    {
        _keyActionAccessibility = keyActionAccessibility;
        _keyActionControls      = keyActionControls;
        _keyActionRegistrar     = keyActionRegistrar;
        _keyActionUtility       = keyActionUtility;
        _scriptAutoClick        = scriptAutoClick;
        _scriptAutoSkip         = scriptAutoSkip;
    }

    public void Dispose()
    {
        _keyActionControls.Dispose();
        _keyActionRegistrar.Dispose();
        _scriptAutoClick.Dispose();
        _scriptAutoSkip.Dispose();
        _keyActionUtility.Dispose();
        _keyActionAccessibility.Dispose();
    }
}
