namespace AuroraDialogEnhancer.Backend.CursorPositioning;

public class CursorPositionInfo
{
    public int ClosestUpperIndex   { get; }
    public int HighlightedIndex    { get; set; }
    public int ClosestLowerIndex   { get; }
    public bool IsWithinBoundaries { get; }

    public CursorPositionInfo(int closestUpperIndex, int highlightedIndex, int closestLowerIndex)
    {
        ClosestUpperIndex  = closestUpperIndex;
        HighlightedIndex   = highlightedIndex;
        ClosestLowerIndex  = closestLowerIndex;
        IsWithinBoundaries = closestUpperIndex != -1 || highlightedIndex != -1 || closestLowerIndex != -1;
    }
}
