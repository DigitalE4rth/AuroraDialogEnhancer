using System;
using System.Drawing;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class CvPreset : IDisposable
{
    #region General
    public Size Resolution { get; set; }
    #endregion

    #region Search regions
    public Rectangle SpeakerNameSearchRegion { get; set; }

    public ColorRangeScalar? SpeakerNameColorRange { get; set; }
    #endregion

    #region Dialog option
    public Rectangle DialogOptionSearchRegion { get; set; }

    public double Threshold { get; set; }

    public DialogOptionTemplate? DialogOptionTemplate { get; set; }

    public Rectangle DialogOptionRegion { get; set; }

    public Point DialogOptionInitialCursorPosition { get; set; }

    public int DialogOptionGap { get; set; }
    #endregion

    #region Buttons
    public Point AutoSkipLocation { get; set; }

    public Point HideUiLocation { get; set; }
    #endregion

    #region Utils
    public Point HiddenCursorLocation { get; set; }
    public Point FullScreenPopUpLocation { get; set; }
    #endregion

    public void Dispose()
    {
        DialogOptionTemplate?.Dispose();
    }
}
