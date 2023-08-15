using System.Collections.Generic;
using AuroraDialogEnhancerExtensions;

namespace AuroraDialogEnhancer.Backend.Extensions;

public class ExtensionsProvider
{
    public Dictionary<string, ExtensionDto> ExtensionsDictionary { get; } = new();

    public void Initialize(IEnumerable<ExtensionDto> extensions)
    {
        foreach (var extension in extensions)
        {
            ExtensionsDictionary.Add(extension.Id, extension);
        }
    }
}
