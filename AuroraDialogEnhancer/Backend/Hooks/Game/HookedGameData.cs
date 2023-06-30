﻿using System;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using AuroraDialogEnhancer.Backend.OpenCv;

namespace AuroraDialogEnhancer.Backend.Hooks.Game;

public class HookedGameData : IDisposable
{
    public ExtensionConfig? ExtensionConfig { get; set; }

    public System.Diagnostics.Process? GameProcess { get; set; }

    public WindowInfo? GameWindowInfo { get; set; }

    public CvPreset? CvPreset { get; set; }

    public void Dispose()
    {
        GameProcess?.Dispose();
        CvPreset?.Dispose();
    }
}
