using System.Drawing;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Content;

public abstract class CvPresetDto
{
    #region General
    public virtual Size Resolution { get; protected set; }
    #endregion

    #region Search regions
    public virtual Rectangle SpeakerNameSearchRegion { get; protected set; }

    public virtual ColorRange SpeakerNameColorRange { get; protected set; } = new();
    #endregion

    #region Dialog option
    public virtual Rectangle DialogOptionSearchRegion { get; protected set; }

    public virtual double Threshold { get; protected set; } = 0.7;

    public virtual Bitmap GetDialogOptionTemplate() => new(0,0);

    public virtual Bitmap GetDialogOptionMask() => new(0, 0);

    public virtual Rectangle DialogOptionRegion { get; protected set; }

    public virtual Point DialogOptionInitialCursorPosition { get; protected set; }

    public virtual int DialogOptionGap { get; protected set; }
    #endregion

    #region Buttons
    public virtual Point AutoSkipLocation { get; protected set; }

    public virtual Point HideUiLocation { get; protected set; }

    public virtual Point FullScreenPopUpLocation { get; protected set; }
    #endregion

    #region Utils
    public virtual Point HiddenCursorLocation { get; protected set; }
    #endregion
}
