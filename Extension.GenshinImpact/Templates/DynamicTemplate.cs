using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate
{
    /// <summary>
    /// The color range of the speaker name.
    /// </summary>
    public ColorRange SpeakerColorRange = new(new Rgba(255, 230, 170, 0), new Rgba(255, 255, 210, 10));

    /// <summary>
    /// The speaker name area.
    /// </summary>
    public AreaDynamic SpeakerNameArea = new(0.485, 0.515, 0.8, 0.9);

    /// <summary>
    /// The percentage of pixels in the color range of the total number of pixels.
    /// </summary>
    public double SpeakerNameThreshold = 0.1;

    /// <summary>
    /// The search area of the dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public AreaDynamic DialogOptionsSearchArea = new(0.66, 0.692, 0, 1);

    /// <summary>
    /// The search range of the vertical outline line by X.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option width.
    /// </remarks>
    public RangeDynamic VerticalOutlineSearchRangeX = new(0, 0.1);

    /// <summary>
    /// The search range of the vertical outline line by Y.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option height.
    /// </remarks>
    public RangeDynamic VerticalOutlineSearchRangeY = new(0.47, 0.52);

    /// <summary>
    /// The search range of the horizontal outline line by X.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option width.
    /// </remarks>
    public RangeDynamic HorizontalOutlineSearchRangeX = new(0.5, 1);

    /// <summary>
    /// The gray color range of the outline area.
    /// </summary>
    public ChannelRange GrayChannelRange = new(76, 109);

    /// <summary>
    /// The width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public double Width = 0.29;

    /// <summary>
    /// The height of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public double Height = 0.033;

    /// <summary>
    /// The gap between dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public double Gap = 0.0056;

    /// <summary>
    /// The area between the edge of the dialog option and the outline area.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public double BackgroundPadding = 0.0015;

    /// <summary>
    /// The height of the outline area.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option area.
    /// </remarks>
    public double OutlineAreaHeight = 0.95;

    /// <summary>
    /// The search area of the topmost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public RangeDynamic TopOutlineSearchRangeY = new(0, 0.1);

    /// <summary>
    /// The search area of the center outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public RangeDynamic CenterOutlineSearchRangeY = new(0.48, 0.52);

    /// <summary>
    /// The search area of the bottommost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public RangeDynamic BottomOutlineSearchRangeY = new(0.95, 1);

    /// <summary>
    /// The threshold of the dialog options finder service.
    /// </summary>
    public double Threshold = 0.9;
}
