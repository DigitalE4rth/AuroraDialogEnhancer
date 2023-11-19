using System;
using System.Reflection;

namespace AuroraDialogEnhancer.AppConfig.Statics;

internal class AssemblyInfo
{
    public string Name => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

    public Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public string VersionString => Version.ToString();
}
