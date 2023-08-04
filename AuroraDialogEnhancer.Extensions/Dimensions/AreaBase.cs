namespace AuroraDialogEnhancerExtensions.Dimensions;

public abstract class AreaBase<T>
{
    public T Width { get; }
    public T Height { get; }

    protected AreaBase(T width, T height)
    {
        Width = width;
        Height = height;
    }
}
