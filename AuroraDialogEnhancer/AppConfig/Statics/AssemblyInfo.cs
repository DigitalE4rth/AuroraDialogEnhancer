using System.Reflection;

namespace AuroraDialogEnhancer.AppConfig.Statics;

internal class AssemblyInfo
{
    public string Name => Assembly.GetExecutingAssembly().GetName().Name;

    public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
}
