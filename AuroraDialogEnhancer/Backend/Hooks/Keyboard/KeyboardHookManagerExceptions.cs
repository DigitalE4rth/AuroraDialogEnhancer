using System;

namespace AuroraDialogEnhancer.Backend.Hooks.Keyboard;

public class NonInvasiveKeyboardHookException : Exception
{
}

public class HotkeyAlreadyRegisteredException : NonInvasiveKeyboardHookException
{
}

public class HotkeyNotRegisteredException : NonInvasiveKeyboardHookException
{
}
