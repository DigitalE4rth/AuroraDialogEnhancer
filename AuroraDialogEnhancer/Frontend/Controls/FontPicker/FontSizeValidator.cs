using System.Globalization;
using System.Windows.Controls;

namespace AuroraDialogEnhancer.Frontend.Controls.FontPicker;

internal class FontSizeValidator : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var isParsed = double.TryParse((string)value, out var result);
        if (!isParsed || result is < 8 or > 24) return new ValidationResult(false, string.Empty);
        return ValidationResult.ValidResult;
    }
}
