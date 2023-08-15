using System.Drawing;

namespace AuroraDialogEnhancerExtensions.Proxy;

public interface IPresetProvider
{
    public IPresetDto GetPreset(Size clientSize);
}