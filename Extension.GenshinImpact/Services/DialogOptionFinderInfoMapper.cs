using System.Drawing;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.Presets;
using Extension.GenshinImpact.Templates;

namespace Extension.GenshinImpact.Services;

public class DialogOptionFinderInfoMapper
{
    public DialogOptionFinderProvider Map(Size clientSize)
    {
        var searchTemplate = new SearchTemplateMapper().Map(clientSize);
        var dialogOptionsFinder = new DialogOptionFinder(searchTemplate);

        var speakerNameArea = new Rectangle(
            searchTemplate.SpeakerNameArea.Width.From,
            searchTemplate.SpeakerNameArea.Height.From,
            searchTemplate.SpeakerNameArea.Width.Length,
            searchTemplate.SpeakerNameArea.Height.Length);

        var captureArea = new Rectangle(
            searchTemplate.TemplateSearchArea.Width.From,
            searchTemplate.TemplateSearchArea.Height.From,
            searchTemplate.TemplateSearchArea.Width.Length,
            searchTemplate.TemplateSearchArea.Height.Length);

        var presetData = new PresetData();
        var initialCursorPosition = new Point(
            (int) (searchTemplate.DialogOptionWidth * presetData.InitialCursorPosition.X),
            (int)(searchTemplate.DialogOptionHeight * presetData.InitialCursorPosition.Y));

        var dialogOptionFinderData = new DialogOptionFinderData(speakerNameArea, captureArea, initialCursorPosition);
        
        return new DialogOptionFinderProvider(dialogOptionsFinder, dialogOptionFinderData);
    }
}
