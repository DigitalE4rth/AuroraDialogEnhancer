using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Process;

namespace AuroraDialogEnhancer.Backend.Hooks.WindowGi;

public class MinimizationHookGiService : ProcessHookBase
{
    private readonly ProcessDataProvider _processDataProvider;
    
    protected override uint ProcessId => (uint) _processDataProvider.Data!.GameProcess!.Id;
    
    /// <summary>
    /// A window object is about to be minimized. This event is sent by the system, never by servers.
    /// </summary>
    public override uint EventMin => 0x0016;
    
    /// <summary>
    /// A window object is about to be restored. This event is sent by the system, never by servers.
    /// </summary>
    public override uint EventMax => 0x0017;

    public MinimizationHookGiService(ProcessDataProvider processDataProvider)
    {
        _processDataProvider = processDataProvider;
    }
}
