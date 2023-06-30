namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class DialogOptionCursorPositionInfo
{
    public DialogOptionCursorPositionInfo(int closestUpperIndex, int highlightedIndex, int closestLowerIndex)
    {
        ClosestUpperIndex  = closestUpperIndex;
        HighlightedIndex   = highlightedIndex;
        ClosestLowerIndex  = closestLowerIndex;
        IsWithinBoundaries = closestUpperIndex != -1 || highlightedIndex != -1 || closestLowerIndex != -1;
    }

    public int ClosestUpperIndex { get; }

    public int HighlightedIndex { get; set; }

    public int ClosestLowerIndex { get; }

    public bool IsWithinBoundaries { get; }
}
