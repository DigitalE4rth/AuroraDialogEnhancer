namespace AuroraDialogEnhancer.Frontend.Forms.Debug.CvData;

internal class SearchTemplate
{
    /// <summary>
    /// The search area of the dialog options.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public SearchArea<int> DialogOptionsSearchArea { get; set; } = new();

    /// <summary>
    /// The search range of the vertical outline line by X.
    /// </summary>
    /// <remarks>
    /// Relative to the dialog option height.
    /// </remarks>
    public SearchRange<int> VerticalOutlineSearchRangeX { get; set; } = new();

    /// <summary>
    /// The height of the vertical outline line.
    /// </summary>
    /// <remarks>
    /// Relative to the outline area height.
    /// </remarks>
    public int VerticalOutlineSearchHeight = 5;

    /// <summary>
    /// The search range of the horizontal outline line by X.
    /// </summary>
    /// <remarks>
    /// Relative to the dialog option width.
    /// </remarks>
    public SearchRange<int> HorizontalOutlineSearchRangeX { get; set; } = new();

    /// <summary>
    /// The gray color range of the outline area.
    /// </summary>
    public SearchRange<int> GrayColorRange { get; set; } = new();

    /// <summary>
    /// The width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public int Width { get; set; }

    /// <summary>
    /// The height of the dialog option.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public int Height { get; set; }

    /// <summary>
    /// The gap between dialog options.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public int Gap { get; set; }

    /// <summary>
    /// The area between the edge of the dialog option and the outline area.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public int BackgroundPadding { get; set; }

    /// <summary>
    /// The height of the outline area.
    /// </summary>
    /// <remarks>
    /// Relative to the dialog option area.
    /// </remarks>
    public int OutlineAreaHeight { get; set; }

    /// <summary>
    /// The search area of the topmost outline line.
    /// </summary>
    /// <remarks>
    /// Relative to the outline area.
    /// </remarks>
    public SearchRange<int> TopOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// The search area of the center outline line.
    /// </summary>
    /// <remarks>
    /// Relative to the outline area.
    /// </remarks>
    public SearchRange<int> CenterOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// The search area of the bottommost outline line.
    /// </summary>
    /// <remarks>
    /// Relative to the outline area.
    /// </remarks>
    public SearchRange<int> BottomOutlineSearchRangeY { get; set; } = new();

    /// <summary>
    /// The offset of the dialog option by X and Y.
    /// </summary>
    public (int, int) Offset { get; set; }
}
