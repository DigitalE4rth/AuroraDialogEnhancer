using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;
using Extension.HonkaiStarRail.Utils;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplateBase
{
    #region Dialog

    /// <summary>
    /// The color range of the dialog indication.
    /// </summary>
    public virtual ColorRange[] DialogIndicationColorRange => new ColorRange[]
    {
        new(new Rgba(255, 237, 210, 150), new Rgba(255, 245, 216, 156)),
        new(new Rgba(255, 212, 186, 133), new Rgba(255, 216, 204, 144)),
    };

    /// <summary>
    /// Area with an indication of dialogue mode
    /// </summary>
    public virtual AreaDynamic DialogIndicationArea => new(0.07, 0.086, 0.041, 0.068);

    /// <summary>
    /// The color range of the speaker name.
    /// </summary>
    public virtual ColorRange SpeakerColorRange => new(new Rgba(255, 216, 193, 143), new Rgba(255, 220, 198, 148));

    /// <summary>
    /// The speaker name area.
    /// </summary>
    public virtual AreaDynamic SpeakerNameArea => new(0.485, 0.515, 0.70, 0.9);

    /// <summary>
    /// The percentage of pixels in the color range of the total number of pixels.
    /// </summary>
    public virtual double SpeakerNameThreshold => 0.005;
    #endregion

    #region Measurements
    /// <summary>
    /// The search area of the dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public virtual AreaDynamic TemplateSearchArea => new(0.66, 0.796875, 0, 1);

    /// <summary>
    /// The width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public virtual double DialogOptionWidth => 0.2275;

    /// <summary>
    /// The gap between dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public virtual double Gap => 0.01;
    #endregion

    #region Icon
    /// <summary>
    /// The horizontal search range of the icon.
    /// </summary>
    /// <remarks>
    /// Related to the client width.
    /// </remarks>
    public virtual RangeDynamic IconHorizontalRange => new(0.6765625, 0.703125);

    /// <summary>
    /// The minimum size of the icon.
    /// </summary>
    public virtual double IconMinLength => 0.007;

    /// <summary>
    /// The maximum size of the icon.
    /// </summary>
    public virtual double IconMaxLength => 0.02084;

    /// <summary>
    /// The minimum percentage of matching pixels that exceeds the maximum outline color range in the icon area.
    /// </summary>
    public virtual double IconThreshold => 0.15;

    /// <summary>
    /// An empty area above and below the icon.
    /// </summary>
    public virtual double IconTopBottomMargin => 0.008;

    /// <summary>
    /// The offset in pixels for the area in which the determination is made whether there are pixels with a color less than the icon color.
    /// </summary>
    public virtual int IconClearAreaSearchOffset => 2;
    #endregion

    #region Text
    /// <summary>
    /// The horizontal search range of the text.
    /// </summary>
    /// <remarks>
    /// Related to the client width.
    /// </remarks>
    public virtual RangeDynamic TextHorizontalRange => new(0.71015, 0.796875);

    /// <summary>
    /// The text line height.
    /// </summary>
    /// <remarks>
    /// Related to the client height.
    /// </remarks>
    public virtual double TextLineHeight => 0.0208;

    /// <summary>
    /// An empty area above and below the text in the dialog option, that has <b>SINGLE</b> text line.
    /// </summary>
    /// <remarks>
    /// Related to the <see cref="TextLineHeight"/>
    /// </remarks>
    public virtual double TextSingleTopBottomMargin => 0.55;

    /// <summary>
    /// An empty area above and below the text in the dialog option, that has <b>SEVERAL</b> text lines.
    /// </summary>
    /// <remarks>
    /// Related to the <see cref="TextLineHeight"/>
    /// </remarks>
    public virtual double TextMultipleTopBottomMargin => 0.36;
    #endregion

    #region Colors
    public virtual ColorWrapper[] DialogOptionColorRanges => new ColorWrapper[]
    {
        // White Unread 
        new(new ColorRange(new Rgba(255, 252, 252, 252), new Rgba(255, 255, 255, 255)),
            new ColorRange[]
            {
                new(new Rgba(255, 250, 250, 250), new Rgba(255, 255, 255, 255)),
                new(new Rgba(255, 242, 242, 242), new Rgba(255, 250, 250, 250)),
            }),

        // Dimmed White Read 
        new(new ColorRange(new Rgba(255, 129, 129, 129), new Rgba(255, 145, 145, 145)),
            new ColorRange[]
            {
                new(new Rgba(255, 110, 110, 110), new Rgba(255, 127, 127, 127))
            }),

        // Highlighted
        new(new ColorRange(new Rgba(255, 227, 196, 138), new Rgba(255, 233, 210, 148)),
            new ColorRange[]
            {
                new(new Rgba(255, 199, 175, 130), new Rgba(255, 222, 197, 148))
            })
    };
    #endregion
}
