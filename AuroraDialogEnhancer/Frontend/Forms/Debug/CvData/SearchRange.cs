namespace AuroraDialogEnhancer.Frontend.Forms.Debug.CvData;

internal class SearchRange<T>
{
    public T From { get; } = default!;
    public T To { get; } = default!;

    public SearchRange(T from, T to)
    {
        From = from;
        To = to;
    }

    public SearchRange()
    {
    }
}
