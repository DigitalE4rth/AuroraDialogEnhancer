using System.Drawing;

namespace Extension.GenshinImpact.Presets;

public sealed class Preset1920X1080 : GenshinImpactCvPreset
{
    #region General

    public override Size Resolution { get; protected set; } = new(1920, 1080);
    #endregion

    #region Search regions
    public override Rectangle SpeakerNameSearchRegion { get; protected set; } = new(800, 750, 300, 300);
    #endregion

    #region Dialog option
    public override Rectangle DialogOptionSearchRegion { get; protected set; } = new(960, 0, 960, 1080);

    public override Bitmap GetDialogOptionTemplate() => Properties.Resources.Template_1920;

    public override Bitmap GetDialogOptionMask() => Properties.Resources.Mask_1920;

    public override Rectangle DialogOptionRegion { get; protected set; } = new(0, -1, 575, 64);

    public override Point DialogOptionInitialCursorPosition { get; protected set; } = new(84, 55);

    public override int DialogOptionGap { get; protected set; } = 10;
    #endregion

    #region Buttons
    public override Point AutoSkipLocation { get; protected set; } = new(100, 50);

    public override Point FullScreenPopUpLocation { get; protected set; } = new(1280, 1040);
    #endregion

    #region Utils
    public override Point HiddenCursorLocation { get; protected set; } = new(1915, 1078);
    #endregion
}
