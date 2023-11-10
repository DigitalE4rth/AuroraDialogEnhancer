using AuroraDialogEnhancerExtensions.Dimensions;
using System.Collections.Generic;

namespace Extension.GenshinImpact.Templates;

public class PreciseTemplate
{
    #region Speaker
    /// <summary>
    /// The color range of the speaker name.
    /// </summary>
    public ColorRange SpeakerColorRange = new(new Rgba(255, 230, 170, 0), new Rgba(255, 255, 210, 10));

    /// <summary>
    /// The speaker name area
    /// </summary>
    public Area SpeakerNameArea = new();

    /// <summary>
    /// Minimum number of pixels in the color range.
    /// </summary>
    public int SpeakerNameThreshold;
    #endregion

    #region Measurements
    /// <summary>
    /// The search area of the dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public Area TemplateSearchArea { get; set; } = new();

    /// <summary>
    /// The width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public int DialogOptionWidth { get; set; }

    /// <summary>
    /// The height of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public int DialogOptionHeight { get; set; }

    /// <summary>
    /// The gap between dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public int Gap { get; set; }

    /// <summary>
    /// The area between the edge of the dialog option and the outline area.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public int BackgroundPadding { get; set; }

    /// <summary>
    /// The height of the outline area.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option area.
    /// </remarks>
    public int OutlineAreaHeight { get; set; }
    #endregion

    #region Lines
    /// <summary>
    /// The gray color range of the outline area.
    /// </summary>
    public ChannelRange OutlineGrayChannelRange { get; set; } = new();

    /// <summary>
    /// The search range of the vertical outline line by X.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option width.
    /// </remarks>
    public Range VerticalOutlineSearchRangeX { get; set; } = new();

    /// <summary>
    /// The search range of the vertical outline line by Y.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option height.
    /// </remarks>
    public Range VerticalOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// The threshold of the vertical outline line.
    /// </summary>
    public int VerticalOutlineThreshold;

    /// <summary>
    /// The search range of the horizontal outline line by X.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option width.
    /// </remarks>
    public Range HorizontalOutlineSearchRangeX { get; set; } = new();

    /// <summary>
    /// The threshold of the horizontal outline line.
    /// </summary>
    public int HorizontalOutlineThreshold;

    /// <summary>
    /// The search area of the topmost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public Range TopOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// The search area of the bottommost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public Range BottomOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// Corner outline areas.
    /// </summary>
    /// <remarks>
    /// Related to the template sizes.
    /// </remarks>
    public List<ThresholdArea> CornerOutlineAreas = new();
    #endregion

    #region Extra
    /// <summary>
    /// The search area of the icon.
    /// </summary>
    /// <remarks>
    /// Related to the width of the dialog option search area ant to the height of the outline area.
    /// </remarks>
    public Area IconArea = new();

    public List<ColorRange> IconColorRanges = new(0);

    /// <summary>
    /// The minimum number of matching pixels that exceeds the maximum outline color range in the icon area.
    /// </summary>
    public int IconAreaThreshold;

    /// <summary>
    /// The center area of the dialog option without outline pixels.
    /// </summary>
    /// <remarks>
    /// Related to the width of the dialog option search area ant to the height of the outline area.
    /// </remarks>
    public Area EmptyCenterArea = new();

    /// <summary>
    /// The minimum number of matching pixels outside the outline color range.
    /// </summary>
    public int EmptyCenterAreaThreshold;
    #endregion
}
