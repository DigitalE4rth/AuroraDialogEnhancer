using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.AppConfig.Updater;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyHandler2.Scripts;
using AuroraDialogEnhancer.Backend.PeripheralEmulators;
using AuroraDialogEnhancerExtensions.Dimensions;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

public partial class DebugPage
{
    public DebugPage()
    {
        InitializeComponent();
    }
}
