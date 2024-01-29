using System;
using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;
using Extension.HonkaiStarRail.Utils;

namespace Extension.HonkaiStarRail.Templates;

public class SearchTemplate
{
    #region Dialog

    /// <summary>
    /// The color range of the dialog indication.
    /// </summary>
    public ColorRange<Rgba>[] DialogIndicationColorRange { get; set; } = Array.Empty<ColorRange<Rgba>>();

    /// <summary>
    /// Area with an indication of dialogue mode
    /// </summary>
    public Area DialogIndicationArea { get; set; } = new();

    /// <summary>
    /// The color range of the speaker name.
    /// </summary>
    public ColorRange<Rgba> SpeakerColorRangeRgb { get; set; } = new(new Rgba(), new Rgba());

    /// <summary>
    /// The speaker name area.
    /// </summary>
    public Area SpeakerNameArea { get; set; } = new();
    
    /// <summary>
    /// The percentage of pixels in the color range of the total number of pixels.
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
    public int DialogOptionWidth;

    /// <summary>
    /// The gap between dialog options.
    /// </summary>
    /// <remarks>
    /// Related to the client size.
    /// </remarks>
    public int Gap;

    /// <summary>
    /// The half of the <see cref="Gap"/> height.
    /// </summary>
    public int GapHalf;
    #endregion

    #region Icon
    /// <summary>
    /// The horizontal search range of the icon.
    /// </summary>
    /// <remarks>
    /// Related to the client width.
    /// </remarks>
    public Range IconHorizontalRange { get; set; } = new();

    /// <summary>
    /// The minimum size of the icon.
    /// </summary>
    public int IconMinLength;

    /// <summary>
    /// The maximum size of the icon.
    /// </summary>
    public int IconMaxLength;

    /// <summary>
    /// The minimum percentage of matching pixels that exceeds the maximum outline color range in the icon area.
    /// </summary>
    public double IconThreshold;

    /// <summary>
    /// The offset for the area in which the determination is made whether there are pixels with a color less than the icon color.
    /// </summary>
    /// <value>
    /// Percentage of the calculated icon height.
    /// </value>
    public double IconClearAreaSearchOffset;
    #endregion

    #region Text
    /// <summary>
    /// The horizontal search range of the text.
    /// </summary>
    /// <remarks>
    /// Related to the client width.
    /// </remarks>
    public Range TextHorizontalRange { get; set; } = new();

    /// <summary>
    /// The text line height.
    /// </summary>
    /// <remarks>
    /// Related to the client height.
    /// </remarks>
    public int TextLineHeight;

    /// <summary>
    /// Half the height of the text line height.
    /// </summary>
    public int TextLineHeightHalf;

    /// <summary>
    /// An empty area above and below the text in the dialog option, that has <b>SINGLE</b> text line.
    /// </summary>
    /// <remarks>
    /// Related to the <see cref="TextLineHeight"/>
    /// </remarks>
    public int TextSingleTopBottomMargin;

    /// <summary>
    /// An empty area above and below the text in the dialog option, that has <b>SEVERAL</b> text lines.
    /// </summary>
    /// <remarks>
    /// Related to the <see cref="TextLineHeight"/>
    /// </remarks>
    public int TextMultipleTopBottomMargin;
    #endregion


    #region Colors
    public ColorWrapper<Rgba>[] DialogOptionColorRanges { get; set; } = Array.Empty<ColorWrapper<Rgba>>();

    public ColorWrapper<Hsba> DialogOptionDimmed { get; set; } = new(new ColorRange<Hsba>(new Hsba(), new Hsba()), Array.Empty<ColorRange<Hsba>>());
    #endregion
}
