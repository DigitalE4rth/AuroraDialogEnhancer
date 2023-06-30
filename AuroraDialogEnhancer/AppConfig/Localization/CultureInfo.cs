namespace AuroraDialogEnhancer.AppConfig.Localization;

public class CultureInfo
{
    public CultureInfo(string ietfLanguageTag, string displayName)
    {
        IetfLanguageTag = ietfLanguageTag;
        DisplayName = displayName;
    }

    /// <summary>
    /// Ietf language tag.
    /// </summary>
    public string IetfLanguageTag { get; }

    /// <summary>
    /// Culture full name.
    /// </summary>
    public string DisplayName { get; }

    protected bool Equals(CultureInfo other)
    {
        return IetfLanguageTag == other.IetfLanguageTag && DisplayName == other.DisplayName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CultureInfo)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (IetfLanguageTag.GetHashCode() * 397) ^ DisplayName.GetHashCode();
        }
    }
}
