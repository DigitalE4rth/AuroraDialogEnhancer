using System.Windows;
using System.Windows.Media;
using AuroraDialogEnhancer.AppConfig.Statics;

namespace AuroraDialogEnhancer.Frontend.Forms.Utils;

public class InfoDialogConfig
{
    public string WindowTitle { get; set; } = Global.AssemblyInfo.Name;

    public bool ShowSecondaryButton { get; set; }

    public string PrimaryButtonText { get; set; } = Properties.Localization.Resources.InfoDialog_Ok;

    public string SecondaryButtonText { get; set; } = Properties.Localization.Resources.InfoDialog_Cancel;

    public string Message { get; set; } = string.Empty;

    public bool IsReadOnly { get; set; }

    public PathGeometry? IconData { get; set; }

    public Brush? IconForeground { get; set; }

    public Window? Owner { get; set; } = Application.Current.MainWindow;
}
