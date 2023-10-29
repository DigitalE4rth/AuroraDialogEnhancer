using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplateBase
{
    #region Speaker
    /// <summary>
    /// The color range of the speaker name.
    /// </summary>
    public virtual ColorRange SpeakerColorRange { get; } = new(new Rgba(255, 230, 170, 0), new Rgba(255, 255, 210, 10));

    /// <summary>
    /// The speaker name area.
    /// </summary>
    public virtual AreaDynamic SpeakerNameArea { get; } = new(0.485, 0.515, 0.70, 0.9);

    /// <summary>
    /// The percentage of pixels in the color range of the total number of pixels.
    /// </summary>
    public virtual double SpeakerNameThreshold { get; } = 0.005;
    #endregion

    #region Measurements    
    /// <summary>
    /// The template width.
    /// </summary>
    public virtual double TemplateWidth { get; } = 0.032;

    /// <summary>
    /// The template height.
    /// </summary>
    public virtual double TemplateHeight { get; } = 0.033;

    /// <summary>
    /// The search area of the dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public virtual AreaDynamic TemplateSearchArea { get; } = new(0.66, 0.692, 0, 1);

    /// <summary>
    /// The width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public virtual double DialogOptionWidth { get; } = 0.29;

    /// <summary>
    /// The height of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public virtual double DialogOptionHeight { get; } = 0.033;

    /// <summary>
    /// The gap between dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public virtual double Gap { get; } = 0.0056;

    /// <summary>
    /// The area between the edge of the dialog option and the outline area.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public virtual double BackgroundPadding { get; } = 0.0015;

    /// <summary>
    /// The height of the outline area.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option area.
    /// </remarks>
    public virtual double OutlineAreaHeight { get; } = 0.95;
    #endregion

    #region Lines
    /// <summary>
    /// The gray color range of the outline area.
    /// </summary>
    public virtual ChannelRange OutlineGrayChannelRange { get; } = new(76, 109);

    /// <summary>
    /// The search range of the vertical outline line by X.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option width.
    /// </remarks>
    public virtual RangeDynamic VerticalOutlineSearchRangeX { get; } = new(0, 0.1);

    /// <summary>
    /// The search range of the vertical outline line by Y.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option height.
    /// </remarks>
    public virtual RangeDynamic VerticalOutlineSearchRangeY { get; } = new(0.47, 0.52);

    /// <summary>
    /// The threshold of the vertical outline line.
    /// </summary>
    public virtual double VerticalOutlineThreshold { get; } = 1;

    /// <summary>
    /// The search range of the horizontal outline line by X.
    /// </summary>
    /// <remarks>
    /// Related to the dialog option width.
    /// </remarks>
    public virtual RangeDynamic HorizontalOutlineSearchRangeX { get; } = new(0.5, 1);

    /// <summary>
    /// The threshold of the horizontal outline line.
    /// </summary>
    public virtual double HorizontalOutlineThreshold { get; } = 0.9;

    /// <summary>
    /// The search area of the topmost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public virtual RangeDynamic TopOutlineSearchRangeY { get; } = new(0, 0.1);

    /// <summary>
    /// The search area of the bottommost outline line.
    /// </summary>
    /// <remarks>
    /// Related to the outline area.
    /// </remarks>
    public virtual RangeDynamic BottomOutlineSearchRangeY { get; } = new(0.9, 1);

    /// <summary>
    /// Corner outline areas.
    /// </summary>
    /// <remarks>
    /// Related to the template sizes.
    /// </remarks>
    public virtual List<ThresholdAreaDynamic> CornerOutlineAreas { get; } = new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.05, 0.15, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.13, 0.25, 0.07),
        
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
    public virtual AreaDynamic IconArea { get; } = new(0.20, 0.88, 0.11, 0.89);

    /// <summary>
    /// The minimum percentage of matching pixels that exceeds the maximum outline color range in the icon area.
    /// </summary>
    public virtual double IconAreaThreshold { get; } = 0.2;

    /// <summary>
    /// The center area of the dialog option without outline pixels.
    /// </summary>
    /// <remarks>
    /// Related to the width of the dialog option search area ant to the height of the outline area.
    /// </remarks>
    public virtual AreaDynamic EmptyCenterArea { get; } = new(0.15, 1, 0.1, 0.9);

    /// <summary>
    /// The minimum percentage of matching pixels outside the outline color range.
    /// </summary>
    public virtual double EmptyCenterAreaThreshold { get; } = 0.18;
    #endregion
}
