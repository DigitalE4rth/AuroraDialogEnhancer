using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.InteractionPoints;

public class DynamicPointTemplate
{
    public DynamicPoint AutoPlay { get; set; } = new(0.8956046, 0.0534968);
    public DynamicPoint HideUi { get; set; } = new(0.95222618, 0.0534968);
    public DynamicPoint FullScreenPopUp { get; set; } = new(0.5, 0.8);
}
