using System.Reflection;

namespace AuroraDialogEnhancer.AppConfig.Statics;

internal class AssemblyInfo
{
    public string Name => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

    public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
}
