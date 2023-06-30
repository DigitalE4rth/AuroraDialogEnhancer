using System.Collections.Generic;
using System.Linq;

namespace AuroraDialogEnhancer.AppConfig.Localization;

public class CultureProvider
{
    private const string ResourceVariableName = nameof(Properties.Localization.Resources.Language_Name);
    private const string AutoTagResourceName  = nameof(Properties.Localization.Resources.Language_Automatic);
    private const string AutoTag = "auto";

    private readonly Dictionary<string, CultureInfo> _cultureInfosDict;
    private readonly List<string> _supportedLanguages = new() { "en-US", "ru-RU" };

    public CultureProvider()
    {
        _cultureInfosDict = new Dictionary<string, CultureInfo>
        { 
            { AutoTag, new CultureInfo(AutoTag, $"{Properties.Localization.Resources.ResourceManager.GetString(AutoTagResourceName, System.Globalization.CultureInfo.InstalledUICulture)} ({Properties.Localization.Resources.ResourceManager.GetString(ResourceVariableName, System.Globalization.CultureInfo.InstalledUICulture)})") }
        };

        _supportedLanguages.ForEach(tag => _cultureInfosDict.Add(tag, new CultureInfo(tag, Properties.Localization.Resources.ResourceManager.GetString(ResourceVariableName, new System.Globalization.CultureInfo(tag))!)));
    }

    public bool IsCultureSupported(string ietfLanguageTag) => _cultureInfosDict.ContainsKey(ietfLanguageTag);

    public IList<CultureInfo> GetAvailableCultures() => _cultureInfosDict.Values.ToList();
}
