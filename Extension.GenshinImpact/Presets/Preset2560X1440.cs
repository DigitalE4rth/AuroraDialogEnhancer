using System.Drawing;

namespace Extension.GenshinImpact.Presets;

public sealed class Preset2560X1440 : GenshinImpactCvPreset
{
    #region General

    public override Size Resolution { get; protected set; } = new(2560, 1440);
    #endregion

    #region Search regions
    public override Rectangle SpeakerNameSearchRegion { get; protected set; } = new(1125, 1035, 300, 300);
    #endregion

    #region Dialog option
    public override double Threshold { get; protected set; } = 0.6;

    public override Rectangle DialogOptionSearchRegion { get; protected set; } = new(1689, 235, 83, 1065);

    public override Bitmap GetDialogOptionTemplate() => Properties.Resources.Template_2560;

    public override Bitmap GetDialogOptionMask() => Properties.Resources.Mask_2560;

    public override Rectangle DialogOptionRegion { get; protected set; } = new(-1, -3, 767, 86);

    public override Point DialogOptionInitialCursorPosition { get; protected set; } = new(110, 75);

    public override int DialogOptionGap { get; protected set; } = 12;
    #endregion

    #region Buttons
    public override Point AutoSkipLocation { get; protected set; } = new(100, 50);

    public override Point FullScreenPopUpLocation { get; protected set; } = new(1280, 1400);
    #endregion

    #region Utils
    public override Point HiddenCursorLocation { get; protected set; } = new(2555, 1438);
    #endregion
}
