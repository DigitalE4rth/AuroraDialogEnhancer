using System.Globalization;
using System.Windows.Controls;

namespace AuroraDialogEnhancer.Frontend.Validators;

internal class UnsignedIntegerValidator : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        return uint.TryParse((string) value, out _) 
            ? ValidationResult.ValidResult 
            : new ValidationResult(false, null);
    }
}
