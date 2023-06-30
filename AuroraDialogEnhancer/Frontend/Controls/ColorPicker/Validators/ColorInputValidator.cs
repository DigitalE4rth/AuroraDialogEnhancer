using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Validators;

internal class ColorInputValidator : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        try
        {
            ColorConverter.ConvertFromString((string) value);
            return ValidationResult.ValidResult;
        }
        catch (Exception e)
        {
            return new ValidationResult(false, e);
        }
    }
}
