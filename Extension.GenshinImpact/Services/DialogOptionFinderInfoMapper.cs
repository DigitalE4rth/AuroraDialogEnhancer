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

        var presetConfig = new PresetConfig();
        presetConfig.CursorPositionData.InitialPositionX = (int) (presetConfig.CursorPositionData.InitialPosition.X * searchTemplate.DialogOptionWidth);

        var presetData = new PresetData(speakerNameArea, captureArea, presetConfig.CursorPositionData);
        
        return new DialogOptionFinderProvider(dialogOptionsFinder, presetData);
    }
}
