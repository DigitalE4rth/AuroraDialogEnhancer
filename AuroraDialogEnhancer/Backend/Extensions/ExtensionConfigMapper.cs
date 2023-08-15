using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancerExtensions;
using AuroraDialogEnhancerExtensions.Content;

namespace AuroraDialogEnhancer.Backend.Extensions;

public class ExtensionConfigMapper : IMapper<ExtensionDto, ExtensionConfig>
{
    public ExtensionConfig Map(ExtensionDto obj)
    {
        var config = obj.GetConfig();

        return new ExtensionConfig
        {
            Id   = obj.Id,
            Name = obj.Name,
            DisplayName = obj.DisplayName,

            GameProcessName     = config.GameProcessName,
            LauncherProcessName = config.LauncherProcessName
        };
    }
}
