using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.AppConfig.Updater;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Global;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.PeripheralEmulators;
using AuroraDialogEnhancerExtensions.Dimensions;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

public partial class DebugPage
{
    private readonly GlobalFocusService _globalFocusHook = AppServices.ServiceProvider.GetRequiredService<GlobalFocusService>();

    public DebugPage()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        
        //var core = AppServices.ServiceProvider.GetRequiredService<CoreService>();
        _globalFocusHook.SetWinEventHook();
    }

    private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
    {
        _globalFocusHook.UnhookWinEvent();
    }
}
