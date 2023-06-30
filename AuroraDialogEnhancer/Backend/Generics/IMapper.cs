namespace AuroraDialogEnhancer.Backend.Generics;

public interface IMapper<I, O> where I : class where O : class
{
    public O Map(I obj);
}
