using System.Drawing;

namespace Extension.GenshinImpact.Presets;

public sealed class Preset1680X1050 : GenshinImpactCvPreset
{
    #region General

    public override Size Resolution { get; protected set; } = new(1680, 1050);
    #endregion

    #region Search regions
    public override Rectangle SpeakerNameSearchRegion { get; protected set; } = new(815, 750, 50, 200);
    #endregion

    #region Dialog option
    public override Rectangle DialogOptionSearchRegion { get; protected set; } = new(1108, 195, 6, 700);

    public override double Threshold { get; protected set; } = 0.1;

    public override Bitmap GetDialogOptionTemplate() => Properties.Resources.Template_1680;

    public override Bitmap GetDialogOptionMask() => Properties.Resources.Template_1680;

    public override Rectangle DialogOptionRegion { get; protected set; } = new(-1, -1, 528, 55);

    public override Point DialogOptionInitialCursorPosition { get; protected set; } = new(70, 50);

    public override int DialogOptionGap { get; protected set; } = 10;
    #endregion

    #region Buttons
    public override Point AutoSkipLocation { get; protected set; } = new(60, 35);

    public override Point FullScreenPopUpLocation { get; protected set; } = new(1640, 525);
    #endregion

    #region Utils
    public override Point HiddenCursorLocation { get; protected set; } = new(1675, 1048);
    #endregion
}
