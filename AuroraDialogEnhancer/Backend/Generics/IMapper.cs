namespace AuroraDialogEnhancer.Backend.Generics;

public interface IMapper<in I, O> where O : class
{
    O Map(I obj);
}
