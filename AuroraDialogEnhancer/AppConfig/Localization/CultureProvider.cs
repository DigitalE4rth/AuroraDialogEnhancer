using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AuroraDialogEnhancer.AppConfig.Localization;

public class CultureProvider
{
    private const string RESOURCE_VARIABLE_NAME = nameof(Properties.Localization.Resources.Language_Name);
    private const string AUTO_TAG_RESOURCE_NAME = nameof(Properties.Localization.Resources.Language_Automatic);
    private const string AUTO_TAG = "auto";

    private readonly Dictionary<string, CultureInfo> _cultureInfosDict;
    private readonly List<string> _supportedLanguages = new()
    {
        "en-US", 
        // "de-DE", 
        // "nl-NL", 
        "ru-RU", 
        "uk-UK"
    };

    public CultureProvider()
    {
        var windowsUiCulture = System.Globalization.CultureInfo.GetCultureInfo(GetUserDefaultUILanguage());

        _cultureInfosDict = new Dictionary<string, CultureInfo>
        { 
            { AUTO_TAG, new CultureInfo(AUTO_TAG, $"{Properties.Localization.Resources.ResourceManager.GetString(AUTO_TAG_RESOURCE_NAME, windowsUiCulture)} ({Properties.Localization.Resources.ResourceManager.GetString(RESOURCE_VARIABLE_NAME, windowsUiCulture)})") }
        };

        _supportedLanguages.ForEach(tag => _cultureInfosDict.Add(tag, new CultureInfo(tag, Properties.Localization.Resources.ResourceManager.GetString(RESOURCE_VARIABLE_NAME, new System.Globalization.CultureInfo(tag))!)));
    }

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    private static extern ushort GetUserDefaultUILanguage();

    public bool IsCultureSupported(string ietfLanguageTag) => _cultureInfosDict.ContainsKey(ietfLanguageTag);

    public IList<CultureInfo> GetAvailableCultures() => _cultureInfosDict.Values.ToList();
}
