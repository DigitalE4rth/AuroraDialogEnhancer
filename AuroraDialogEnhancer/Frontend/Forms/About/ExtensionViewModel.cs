using AuroraDialogEnhancerExtensions;
using AuroraDialogEnhancerExtensions.Content;

namespace AuroraDialogEnhancer.Frontend.Forms.About;

public class ExtensionViewModel
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Author { get; set; }

    public string? Link { get; set; }

    public ExtensionViewModel(ExtensionDto extension)
    {
        Title = extension.DisplayName;
        Description = $"{extension.Version} • {extension.Author}";
        Author = extension.Author;
        Link = extension.Link;
    }
}
