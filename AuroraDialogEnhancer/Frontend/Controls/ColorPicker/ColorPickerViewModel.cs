using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Converters;
using AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Services;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker;

internal class ColorPickerViewModel : INotifyPropertyChanged
{
    private readonly HsbColorService     _hsbColorService;
    private readonly HsvToColorConverter _hsvToColorConverter;

    private Color  _color;

    private double _hue;
    private double _saturation;
    private double _brightness;

    private byte   _red;
    private byte   _green;
    private byte   _blue;
    private byte   _alpha;

    // flags to prevent circular updates
    private bool _updateFromColor;
    private bool _updateFromComponents;
    
    public ColorPickerViewModel(Color startUpColor)
    {
        _color = startUpColor;
        _saturation = 1;
        _brightness = 1;
        _alpha = 255;
        _hsbColorService = new HsbColorService();
        _hsvToColorConverter = new HsvToColorConverter();
    }

    public ColorPickerViewModel() : this(Colors.Red)
    {
    }

    public Color Color
    {
        get => _color;
        set
        {
            if (value == _color)
            {
                return;
            }

            _color = value;
            OnPropertyChanged();

            if (!_updateFromComponents)
            {
                UpdateComponents();
            }
        }
    }

    public double Hue
    {
        get => _hue;
        set
        {
            if (Math.Abs(value - _hue) < 0)
            {
                return;
            }

            _hue = value;
            OnPropertyChanged();

            if (!_updateFromColor && !_updateFromComponents)
            {
                UpdateColorFromHsb();
            }
        }
    }

    public double Saturation
    {
        get => _saturation;
        set
        {
            if (Math.Abs(value - _saturation) < 0)
            {
                return;
            }

            _saturation = value;
            OnPropertyChanged();

            if (!_updateFromColor && !_updateFromComponents)
            {
                UpdateColorFromHsb();
            }
        }
    }

    public double Brightness
    {
        get => _brightness;
        set
        {
            if (Math.Abs(value - _brightness) < 0)
            {
                return;
            }

            _brightness = value;
            OnPropertyChanged();

            if (!_updateFromColor && !_updateFromComponents)
            {
                UpdateColorFromHsb();
            }
        }
    }

    public byte Red
    {
        get => _red;
        set
        {
            if (value == _red)
            {
                return;
            }

            _red = value;
            OnPropertyChanged();

            if (!_updateFromColor && !_updateFromComponents)
            {
                UpdateColorFromRgb();
            }
        }
    }

    public byte Green
    {
        get => _green;
        set
        {
            if (value == _green)
            {
                return;
            }

            _green = value;
            OnPropertyChanged();

            if (!_updateFromColor && !_updateFromComponents)
            {
                UpdateColorFromRgb();
            }
        }
    }

    public byte Blue
    {
        get => _blue;
        set
        {
            if (value == _blue)
            {
                return;
            }

            _blue = value;
            OnPropertyChanged();

            if (!_updateFromColor && !_updateFromComponents)
            {
                UpdateColorFromRgb();
            }
        }
    }

    public byte Alpha
    {
        get => _alpha;
        set
        {
            if (value == _alpha)
            {
                return;
            }

            _alpha = value;
            OnPropertyChanged();
            
            if (!_updateFromColor && !_updateFromComponents)
            {
                UpdateColorFromRgb();
            }
        }
    }

    private void UpdateColorFromRgb()
    {
        // When color changes, update hsb
        _updateFromComponents = true;
        Color = Color.FromArgb(Alpha, Red, Green, Blue);
        Hue   = _hsbColorService.GetHue(Color);
        Saturation = _hsbColorService.GetSaturation(Color);
        Brightness = _hsbColorService.GetBrightness(Color);
        _updateFromComponents = false;
    }

    private void UpdateColorFromHsb()
    {
        // When hsb changes, update color
        _updateFromComponents = true;
        var color = _hsvToColorConverter.Convert(Hue, Saturation, Brightness);
        color.A = Alpha;

        Color = color;
        Red   = Color.R;
        Green = Color.G;
        Blue  = Color.B;
        _updateFromComponents = false;
    }

    private void UpdateComponents()
    {
        // When color changes, update hsb and rgb
        _updateFromColor = true;

        Red   = Color.R;
        Green = Color.G;
        Blue  = Color.B;
        Alpha = Color.A;

        Hue = _hsbColorService.GetHue(Color);
        Saturation = _hsbColorService.GetSaturation(Color);
        Brightness = _hsbColorService.GetBrightness(Color);

        _updateFromColor = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
