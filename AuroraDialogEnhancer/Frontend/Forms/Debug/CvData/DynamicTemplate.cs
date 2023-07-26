namespace AuroraDialogEnhancer.Frontend.Forms.Debug.CvData;

internal class DynamicTemplate
{
    /// <summary>
    /// The search area of the dialog options.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public SearchArea<double> DialogOptionsSearchArea = new(0.66, 0.692, 0, 1);

    /// <summary>
    /// The search range of the vertical outline line by X.
    /// </summary>
    /// <remarks>
    /// Relative to the dialog option height.
    /// </remarks>
    public SearchRange<double> VerticalOutlineSearchRangeX = new(0, 0.1);

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
    public SearchRange<double> HorizontalOutlineSearchRangeX = new(0.5, 1);

    /// <summary>
    /// The gray color range of the outline area.
    /// </summary>
    public SearchRange<int> GrayColorRange = new(76, 109);

    /// <summary>
    /// The width of the dialog option.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public double Width = 0.29;

    /// <summary>
    /// The height of the dialog option.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public double Height = 0.033;

    /// <summary>
    /// The gap between dialog options.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public double Gap = 0.0056;

    /// <summary>
    /// The area between the edge of the dialog option and the outline area.
    /// </summary>
    /// <remarks>
    /// Relative to the client size.
    /// </remarks>
    public double BackgroundPadding = 0.0015;

    /// <summary>
    /// The height of the outline area.
    /// </summary>
    /// <remarks>
    /// Relative to the dialog option area.
    /// </remarks>
    public double OutlineAreaHeight = 0.95;

    /// <summary>
    /// The search area of the topmost outline line.
    /// </summary>
    /// <remarks>
    /// Relative to the outline area.
    /// </remarks>
    public SearchRange<double> TopOutlineSearchRangeY = new(0, 0.1);

    /// <summary>
    /// The search area of the center outline line.
    /// </summary>
    /// <remarks>
    /// Relative to the outline area.
    /// </remarks>
    public SearchRange<double> CenterOutlineSearchRangeY = new(0.48, 0.52);

    /// <summary>
    /// The search area of the bottommost outline line.
    /// </summary>
    /// <remarks>
    /// Relative to the outline area.
    /// </remarks>
    public SearchRange<double> BottomOutlineSearchRangeY = new(0.95, 1);
}
