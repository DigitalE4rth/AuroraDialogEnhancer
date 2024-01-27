using System.Drawing;

namespace Extension.HonkaiStarRail.InteractionPoints;

public class InteractionPointsMapper
{
    public PointTemplate Map(Size clientSize)
    {
        var dynamicPointTemplate = new DynamicPointTemplate();
        return new PointTemplate
        {
            AutoPlay = new Point(
                (int)(dynamicPointTemplate.AutoPlay.X * clientSize.Width),
                (int)(dynamicPointTemplate.AutoPlay.Y * clientSize.Height)),
            HideUi = new Point(
                (int)(dynamicPointTemplate.HideUi.X * clientSize.Width),
                (int)(dynamicPointTemplate.HideUi.Y * clientSize.Height)),
            FullScreenPopUp = new Point(
                (int)(dynamicPointTemplate.FullScreenPopUp.X * clientSize.Width),
                (int)(dynamicPointTemplate.FullScreenPopUp.Y * clientSize.Height))
        };
    }
}
