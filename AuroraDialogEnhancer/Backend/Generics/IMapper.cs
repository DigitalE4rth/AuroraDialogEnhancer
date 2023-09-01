namespace AuroraDialogEnhancer.Backend.Generics;

public interface IMapper<in I, O>
{
    O Map(I obj);
}
