using System.Windows;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Forms.Utils;

public class InfoDialogBuilder
{
    private readonly InfoDialogConfig _infoDialogConfig = new();

    public InfoDialogBuilder SetPrimaryButtonText(string text)
    {
        _infoDialogConfig.PrimaryButtonText = text;
        return this;
    }

    public InfoDialogBuilder SetSecondaryButtonText(string text)
    {
        _infoDialogConfig.SecondaryButtonText = text;
        return this;
    }

    public InfoDialogBuilder ShowSecondaryButton()
    {
        _infoDialogConfig.ShowSecondaryButton = true;
        return this;
    }

    public InfoDialogBuilder SetWindowTitle(string text)
    {
        _infoDialogConfig.WindowTitle = text;
        return this;
    }

    public InfoDialogBuilder SetMessage(string? text)
    {
        if (text is null) return this;

        _infoDialogConfig.Message = text;
        return this;
    }

    public InfoDialogBuilder SetMessageReadOnly()
    {
        _infoDialogConfig.IsReadOnly = true;
        return this;
    }

    public InfoDialogBuilder SetCustomIconData(PathGeometry pathGeometry)
    {
        _infoDialogConfig.IconData = pathGeometry;
        return this;
    }

    public InfoDialogBuilder SetCustomIconData(string iconName)
    {
        _infoDialogConfig.IconData = (PathGeometry) Application.Current.Resources[iconName];
        return this;
    }

    public InfoDialogBuilder SetTypeError()
    {
        _infoDialogConfig.IconData = (PathGeometry) Application.Current.Resources["Icon.Warning"];
        return this;
    }

    public InfoDialogBuilder SetIconColor(Brush color)
    {
        _infoDialogConfig.IconForeground = color;
        return this;
    }

    public InfoDialogBuilder SetIconColor(string ahexColor)
    {
        _infoDialogConfig.IconForeground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(ahexColor));
        return this;
    }

    public InfoDialogBuilder SetOwner(Window owner)
    {
        _infoDialogConfig.Owner = owner;
        return this;
    }

    public InfoDialog Build()
    {
        return new InfoDialog(_infoDialogConfig);
    }

    public bool? ShowDialog()
    {
        return Build().ShowDialog();
    }
}
