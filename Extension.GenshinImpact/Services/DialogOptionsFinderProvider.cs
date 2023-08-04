using System.Collections.Generic;
using System.Drawing;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.Templates;

namespace Extension.GenshinImpact.Services;

internal class DialogOptionsFinderProvider : IDialogOptionsFinderProvider
{
    public DialogOptionsFinderInfo GetDialogOptionsFinder(Size clientSize)
    {
        var searchTemplate = new SearchTemplateMapper().Get(clientSize);
        var dialogOptionsFinder = new DialogOptionFinder(searchTemplate);

        var captureArea = new Rectangle(
            searchTemplate.DialogOptionsSearchArea.Width.From,
            searchTemplate.DialogOptionsSearchArea.Height.From,
            searchTemplate.DialogOptionsSearchArea.Width.Length, 
            searchTemplate.DialogOptionsSearchArea.Height.Length);

        return new DialogOptionsFinderInfo(captureArea, dialogOptionsFinder);
    }
}
