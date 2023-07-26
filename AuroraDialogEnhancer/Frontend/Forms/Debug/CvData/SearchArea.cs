namespace AuroraDialogEnhancer.Frontend.Forms.Debug.CvData;

internal class SearchArea<T>
{
    public SearchRange<T> Width { get; } = default!;
    public SearchRange<T> Height { get; } = default!;

    public SearchArea(T widthFrom, T widthTo, T heightFrom, T heightTo)
    {
        Width  = new SearchRange<T>(widthFrom, widthTo);
        Height = new SearchRange<T>(heightFrom, heightTo);
    }

    public SearchArea(SearchRange<T> width, SearchRange<T> height)
    {
        Width  = width;
        Height = height;
    }

    public SearchArea()
    {
    }
}
