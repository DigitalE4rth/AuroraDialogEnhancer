using System.Drawing;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.Templates;

namespace Extension.GenshinImpact.Services;

public class DialogOptionFinderInfoMapper
{
    public DialogOptionFinderInfo Map(Size clientSize)
    {
        var searchTemplate = new SearchTemplateMapper().Map(clientSize);
        var dialogOptionsFinder = new DialogOptionFinder(searchTemplate);

        var captureArea = new Rectangle(
            searchTemplate.DialogOptionsSearchArea.Width.From,
            searchTemplate.DialogOptionsSearchArea.Height.From,
            searchTemplate.DialogOptionsSearchArea.Width.Length,
            searchTemplate.DialogOptionsSearchArea.Height.Length);

        var speakerNameArea = new Rectangle(
            searchTemplate.SpeakerNameArea.Width.From,
            searchTemplate.SpeakerNameArea.Height.From,
            searchTemplate.SpeakerNameArea.Width.Length,
            searchTemplate.SpeakerNameArea.Height.Length);

        return new DialogOptionFinderInfo(speakerNameArea, captureArea, dialogOptionsFinder);
    }
}
