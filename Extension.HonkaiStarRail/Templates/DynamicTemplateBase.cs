using AuroraDialogEnhancerExtensions.Dimensions;
using Extension.HonkaiStarRail.Utils;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplateBase
{
    #region Dialog

    /// <summary>
    /// The color range of the dialog indication.
    /// </summary>
    public virtual ColorRange<Rgba>[] DialogIndicationColorRange => new ColorRange<Rgba>[]
    {
        new(new Rgba(237, 210, 150, 255), new Rgba(245, 216, 156, 255)),
        new(new Rgba(212, 186, 133, 255), new Rgba(216, 204, 144, 255)),
    };

    /// <summary>
    /// Area with an indication of dialogue mode
    /// </summary>
    public virtual AreaDynamic DialogIndicationArea => new(0.07, 0.085, 0.043, 0.066);

    /// <summary>
    /// The color range of the speaker name.
    /// </summary>
    public virtual ColorRange<Rgba> SpeakerColorRangeRgb => new(new Rgba(216, 193, 143, 255), new Rgba(220, 198, 148, 255));

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
    public virtual AreaDynamic TemplateSearchArea => new(0.677, 0.779, 0, 1);

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
    public virtual RangeDynamic IconHorizontalRange => new(0.677, 0.7029);

    /// <summary>
    /// The minimum size of the icon.
    /// </summary>
    public virtual double IconMinLength => 0.007;

    /// <summary>
    /// The maximum size of the icon.
    /// </summary>
    public virtual double IconMaxLength => 0.037;

    /// <summary>
    /// The minimum percentage of matching pixels that exceeds the maximum outline color range in the icon area.
    /// </summary>
    public virtual double IconThreshold => 0.15;

    /// <summary>
    /// The offset for the area in which the determination is made whether there are pixels with a color less than the icon color.
    /// </summary>
    /// <value>
    /// Percentage of the calculated icon height.
    /// </value>
    public virtual double IconClearAreaSearchOffset => 0.12;
    #endregion

    #region Text
    /// <summary>
    /// The horizontal search range of the text.
    /// </summary>
    /// <remarks>
    /// Related to the client width.
    /// </remarks>
    public virtual RangeDynamic TextHorizontalRange => new(0.71, 0.779);

    /// <summary>
    /// The text line height.
    /// </summary>
    /// <remarks>
    /// Related to the client height.
    /// </remarks>
    public virtual double TextLineHeight => 0.029;

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
    public virtual ColorWrapper<Rgba>[] DialogOptionColorRanges => new ColorWrapper<Rgba>[]
    {
        // White Unread 
        new(new ColorRange<Rgba>(new Rgba(252, 252, 252), new Rgba(255, 255, 255)),
            new ColorRange<Rgba>[]
            {
                new(new Rgba(250, 250, 250), new Rgba(255, 255, 255)),
                new(new Rgba(242, 242, 242), new Rgba(250, 250, 250)),
            }),

        // Highlighted
        new(new ColorRange<Rgba>(new Rgba(227, 196, 138), new Rgba(233, 210, 148)),
            new ColorRange<Rgba>[]
            {
                new(new Rgba(199, 175, 130), new Rgba(222, 197, 148))
            })
    };

    public virtual ColorWrapper<Hsba> DialogOptionDimmed => new(
        new ColorRange<Hsba>(new Hsba(0, 0, 0.49), new Hsba(360, 0.08, 0.56)),
        new[]
        {
            new ColorRange<Hsba>(new Hsba(0, 0, 0.48), new Hsba(3, 0.03, 0.52))
        });

    #endregion
}
