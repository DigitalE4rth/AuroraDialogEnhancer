namespace AuroraDialogEnhancer.AppConfig.Localization;

public class CultureService
{
    public readonly CultureProvider CultureProvider;

    public CultureService(CultureProvider cultureProvider)
    {
        CultureProvider = cultureProvider;
    }

    public void Initialize()
    {
        if (Properties.Settings.Default.App_CurrentCulture.Equals("auto")
            && !CultureProvider.IsCultureSupported(System.Globalization.CultureInfo.InstalledUICulture.IetfLanguageTag))
        {
            SetLanguage("en-US");
            return;
        }

        SetLanguage(Properties.Settings.Default.App_CurrentCulture);
    }

    /// <summary>
    /// Concrete language setting method.
    /// </summary>
    /// <param name="ietfLanguageTag">Ietf language tag.</param>
    private void SetLanguage(string ietfLanguageTag)
    {
        var cultureInfo = ietfLanguageTag.Equals("auto")
            ? System.Globalization.CultureInfo.InstalledUICulture
            : new System.Globalization.CultureInfo(ietfLanguageTag);

        System.Globalization.CultureInfo.CurrentCulture   = cultureInfo;
        System.Globalization.CultureInfo.CurrentUICulture = cultureInfo;
    }
}
