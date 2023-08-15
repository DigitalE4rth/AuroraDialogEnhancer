using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class PreciseTemplate
{
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

    /// <summary>
    /// The search area of the dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public Area DialogOptionsSearchArea { get; set; } = new();

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
    /// The search range of the horizontal outline line by X.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option width.
    /// </remarks>
    public Range HorizontalOutlineSearchRangeX { get; set; } = new();

    /// <summary>
    /// The gray color range of the outline area.
    /// </summary>
    public ChannelRange GrayChannelRange { get; set; } = new();

    /// <summary>
    /// The width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public int Width { get; set; }

    /// <summary>
    /// The height of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public int Height { get; set; }

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

    /// <summary>
    /// The search area of the topmost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public Range TopOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// The search area of the center outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public Range CenterOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// The search area of the bottommost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public Range BottomOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// The offset of the dialog option by X and Y.
    /// </summary>
    public (int, int) Offset { get; set; }

    /// <summary>
    /// The threshold of the dialog options finder service.
    /// </summary>
    public double Threshold = 1;
}
