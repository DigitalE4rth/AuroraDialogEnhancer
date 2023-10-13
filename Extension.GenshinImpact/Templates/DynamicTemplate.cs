using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate
{
    #region Speaker
    /// <summary>
    /// The color range of the speaker name.
    /// </summary>
    public ColorRange SpeakerColorRange = new(new Rgba(255, 230, 170, 0), new Rgba(255, 255, 210, 10));

    /// <summary>
    /// The speaker name area.
    /// </summary>
    public AreaDynamic SpeakerNameArea = new(0.485, 0.515, 0.70, 0.9);

    /// <summary>
    /// The percentage of pixels in the color range of the total number of pixels.
    /// </summary>
    public double SpeakerNameThreshold = 0.005;
    #endregion

    #region Measurements    
    /// <summary>
    /// The template width.
    /// </summary>
    public double TemplateWidth = 0.032;

    /// <summary>
    /// The template height.
    /// </summary>
    public double TemplateHeight = 0.033;

    /// <summary>
    /// The search area of the dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public AreaDynamic TemplateSearchArea = new(0.66, 0.692, 0, 1);

    /// <summary>
    /// The width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public double DialogOptionWidth = 0.29;

    /// <summary>
    /// The height of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public double DialogOptionHeight = 0.033;

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
    #endregion

    #region Lines
    /// <summary>
    /// The gray color range of the outline area.
    /// </summary>
    public ChannelRange OutlineGrayChannelRange = new(76, 109);

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
    /// The threshold of the vertical outline line.
    /// </summary>
    public double VerticalOutlineThreshold = 1;

    /// <summary>
    /// The search range of the horizontal outline line by X.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option width.
    /// </remarks>
    public RangeDynamic HorizontalOutlineSearchRangeX = new(0.5, 1);

    /// <summary>
    /// The threshold of the horizontal outline line.
    /// </summary>
    public double HorizontalOutlineThreshold = 0.9;

    /// <summary>
    /// The search area of the topmost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public RangeDynamic TopOutlineSearchRangeY = new(0, 0.1);

    /// <summary>
    /// The search area of the bottommost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public RangeDynamic BottomOutlineSearchRangeY = new(0.9, 1);

    /// <summary>
    /// Corner outline areas.
    /// </summary>
    /// <remarks>
    /// Related to the template sizes.
    /// </remarks>
    public List<ThresholdAreaDynamic> CornerOutlineAreas = new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.05, 0.15, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.125, 0.2, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.3, 0.8, 0.875, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.75, 0.825, 0.07)
    };
    #endregion

    #region Extra
    /// <summary>
    /// The search area of the icon.
    /// </summary>
    /// <remarks>
    /// Related to the width of the dialog option search area ant to the height of the outline area.
    /// </remarks>
    public AreaDynamic IconArea = new(0.20, 0.88, 0.11, 0.89);

    /// <summary>
    /// The minimum percentage of matching pixels that exceeds the maximum outline color range in the icon area.
    /// </summary>
    public double IconAreaThreshold = 0.2;

    /// <summary>
    /// The center area of the dialog option without outline pixels.
    /// </summary>
    /// <remarks>
    /// Related to the width of the dialog option search area ant to the height of the outline area.
    /// </remarks>
    public AreaDynamic EmptyCenterArea = new(0.15, 1, 0.1, 0.9);

    /// <summary>
    /// The minimum percentage of matching pixels outside the outline color range.
    /// </summary>
    public double EmptyCenterAreaThreshold = 0.18;
    #endregion
}
