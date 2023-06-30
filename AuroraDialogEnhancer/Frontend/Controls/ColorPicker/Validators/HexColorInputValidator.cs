using System;
using System.Globalization;
using System.Windows.Controls;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Validators;

internal class HexColorInputValidator : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var convertedValue = value.ToString();
        if (convertedValue[0] != '#')
        {
            convertedValue = convertedValue.Insert(0, "#");
        }

        try
        {
            ColorConverter.ConvertFromString(convertedValue);
            return ValidationResult.ValidResult;
        }
        catch (Exception e)
        {
            return new ValidationResult(false, e);
        }
    }
}
