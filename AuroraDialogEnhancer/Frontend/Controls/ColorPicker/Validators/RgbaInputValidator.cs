using System.Globalization;
using System.Windows.Controls;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Validators;

internal class RgbaInputValidator : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var isParsed = int.TryParse((string)value, out var result);
        if (!isParsed || result is > 255 or < 0) return new ValidationResult(false, string.Empty);
        return ValidationResult.ValidResult;
    }
}
