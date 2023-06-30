using System.Drawing;

namespace Extension.GenshinImpact.Presets;

public sealed class Preset1768X992 : GenshinImpactCvPreset
{
    #region General

    public override Size Resolution { get; protected set; } = new(1768, 992);
    #endregion

    #region Search regions
    public override Rectangle SpeakerNameSearchRegion { get; protected set; } = new(856, 760, 55, 200);
    #endregion

    #region Dialog option
    public override Rectangle DialogOptionSearchRegion { get; protected set; } = new(1167, 90, 57, 870);

    public override double Threshold { get; protected set; } = 0.6;

    public override Bitmap GetDialogOptionTemplate() => Properties.Resources.Template_1768;

    public override Bitmap GetDialogOptionMask() => Properties.Resources.Mask_1768;

    public override Rectangle DialogOptionRegion { get; protected set; } = new(-1, -1, 528, 57);

    public override Point DialogOptionInitialCursorPosition { get; protected set; } = new(70, 47);

    public override int DialogOptionGap { get; protected set; } = 10;
    #endregion

    #region Buttons
    public override Point AutoSkipLocation { get; protected set; } = new(100, 50);

    public override Point FullScreenPopUpLocation { get; protected set; } = new(1728, 520);
    #endregion

    #region Utils
    public override Point HiddenCursorLocation { get; protected set; } = new(1763, 990);
    #endregion
}
