using AuroraDialogEnhancerExtensions.Dimensions;
using Extension.HonkaiStarRail.Utils;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplateBase
{
    #region Dialog Indication
    /// <summary>
    /// Screen area for checking the presence of the dialog mode.
    /// </summary>
    /// <remarks>
    /// Top left "Log" button icon.
    /// </remarks>
    public virtual AreaDynamic DialogIndicationArea => new(0.07, 0.085, 0.043, 0.066);

    /// <summary>
    /// Color ranges for checking the presence of the dialog mode in the <see cref="DialogIndicationArea"/>
    /// </summary>
    /// <remarks>
    /// Colors of the "Log" button icon.
    /// </remarks>
    public virtual ColorRange<Rgba>[] DialogIndicationColorRange => new ColorRange<Rgba>[]
    {
        new(new Rgba(235, 208, 150), new Rgba(245, 216, 156)),
        new(new Rgba(212, 186, 133), new Rgba(216, 204, 144)),
    };

    /// <summary>
    /// Color ranges for checking the presence of the dialog mode in the computed area from <see cref="DialogIndicationArea"/>
    /// </summary>
    /// <remarks>
    /// Colors of the "Log" button icon.
    /// </remarks>
    public virtual ColorRange<Hsba>[] DialogIndicationEmptyColorRange => new ColorRange<Hsba>[]
    {
        new(new Hsba(39, 0.5, 0.7), new Hsba(41, 1, 0.875))
    };
    #endregion



    #region Dialog
    /// <summary>
    /// Color range of the speaker name.
    /// </summary>
    public virtual ColorRange<Rgba> SpeakerColorRange => new(new Rgba(216, 193, 143), new Rgba(220, 198, 148));

    /// <summary>
    /// The speaker name area.
    /// </summary>
    public virtual AreaDynamic SpeakerNameArea => new(0.485, 0.515, 0.70, 0.9);

    /// <summary>
    /// Minimum percentage of matching pixels in the speaker's name area.
    /// </summary>
    public virtual double SpeakerNameThreshold => 0.005;
    #endregion



    #region Measurements
    /// <summary>
    /// Dialog options search area.
    /// </summary>
    public virtual AreaDynamic TemplateSearchArea => new(0.677, 0.779, 0, 1);

    /// <summary>
    /// Width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Related to the client width.
    /// </remarks>
    public virtual double DialogOptionWidth => 0.2275;

    /// <summary>
    /// The gap between dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client height.
    /// </remarks>
    public virtual double Gap => 0.01;
    #endregion



    #region Icon
    /// <summary>
    /// Horizontal search area for dialog option icons.
    /// </summary>
    /// <remarks>
    /// Related to the client width.
    /// </remarks>
    public virtual RangeDynamic IconHorizontalRange => new(0.677, 0.7029);

    /// <summary>
    /// Minimum icon size.
    /// </summary>
    /// <remarks>
    /// Related to the client height.
    /// </remarks>
    public virtual double IconMinLength => 0.007;

    /// <summary>
    /// Maximum icon size.
    /// </summary>
    /// <remarks>
    /// Computed from the maximum icon height. Related to the client height.
    /// </remarks>
    public virtual double IconMaxLength => 0.037;

    /// <summary>
    /// Minimum percentage of matching pixels in the icon area.
    /// </summary>
    public virtual double IconThreshold => 0.15;

    /// <summary>
    /// The offset for the area in which the determination is made whether there are pixels with a color different than the icon color.
    /// </summary>
    /// <value>
    /// Percentage of the calculated icon height.
    /// </value>
    public virtual double IconClearAreaSearchOffset => 0.12;
    #endregion



    #region Text
    /// <summary>
    /// Horizontal search area for dialog option text.
    /// </summary>
    /// <remarks>
    /// Related to the client width.
    /// </remarks>
    public virtual RangeDynamic TextHorizontalRange => new(0.71, 0.779);

    /// <summary>
    /// Text line height.
    /// </summary>
    /// <remarks>
    /// Related to the client height.
    /// </remarks>
    public virtual double TextLineHeight => 0.029;

    /// <summary>
    /// An empty area above and below the text in a dialog option, that has a <b>SINGLE</b> text line.
    /// </summary>
    /// <remarks>
    /// Related to the <see cref="TextLineHeight"/>
    /// </remarks>
    public virtual double TextSingleTopBottomMargin => 0.55;

    /// <summary>
    /// An empty area above and below the text in a dialog option, that has <b>MULTIPLE</b> text lines.
    /// </summary>
    /// <remarks>
    /// Related to the <see cref="TextLineHeight"/>
    /// </remarks>
    public virtual double TextMultipleTopBottomMargin => 0.36;
    #endregion



    #region Colors    
    /// <summary>
    /// Color ranges for icon and text search.
    /// </summary>
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

    /// <summary>
    /// Color ranges for searching icons and text that have muted colors, i.e. that have already been read.
    /// </summary>
    public virtual ColorWrapper<Hsba> DialogOptionMuted => new(
        new ColorRange<Hsba>(new Hsba(0, 0, 0.50), new Hsba(360, 0.0255, 0.56)),
        new[]
        {
            new ColorRange<Hsba>(new Hsba(0, 0, 0.47), new Hsba(0, 0, 0.50))
        });
    #endregion
}
