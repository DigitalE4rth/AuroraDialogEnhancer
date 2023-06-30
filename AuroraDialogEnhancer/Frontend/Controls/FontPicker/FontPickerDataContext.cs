using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuroraDialogEnhancer.Frontend.Controls.FontPicker;

internal class FontPickerDataContext : INotifyPropertyChanged
{
    private double _fontSize = WhyOrchid.Properties.Settings.Default.FontStyle_Medium;

    public double FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
