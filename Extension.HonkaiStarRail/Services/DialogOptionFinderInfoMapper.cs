using System.Drawing;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.HonkaiStarRail.Presets;
using Extension.HonkaiStarRail.Templates;

namespace Extension.HonkaiStarRail.Services;

public class DialogOptionFinderInfoMapper
{
    public DialogOptionFinderProvider Map(Size clientSize)
    {
        var searchTemplate      = new SearchTemplateMapper().Map(clientSize);
        var dialogOptionsFinder = new DialogOptionFinder(searchTemplate);
        //var dialogOptionsFinder = new DialogOptionFinderDebug(searchTemplate);

        var presetConfig = new PresetConfig();
        var dialogConfig = GetDialogDetectionConfig(searchTemplate);
        var cursorConfig = GetCursorConfig(presetConfig, searchTemplate);

        var presetData = new PresetData(dialogConfig, cursorConfig);
        
        return new DialogOptionFinderProvider(dialogOptionsFinder, presetData);
    }

    private DialogDetectionConfig GetDialogDetectionConfig(SearchTemplate searchTemplate)
    {
        var dialogIndicationArea = new Rectangle(
            searchTemplate.DialogIndicationArea.Width.From,
            searchTemplate.DialogIndicationArea.Height.From,
            searchTemplate.DialogIndicationArea.Width.Length,
            searchTemplate.DialogIndicationArea.Height.Length);

        var dialogIndicationAreaEmpty = new Rectangle(
            searchTemplate.DialogIndicationAreaEmpty.Width.From,
            searchTemplate.DialogIndicationAreaEmpty.Height.From,
            searchTemplate.DialogIndicationAreaEmpty.Width.Length,
            searchTemplate.DialogIndicationAreaEmpty.Height.Length);

        var speakerNameArea = new Rectangle(
            searchTemplate.SpeakerNameArea.Width.From,
            searchTemplate.SpeakerNameArea.Height.From,
            searchTemplate.SpeakerNameArea.Width.Length,
            searchTemplate.SpeakerNameArea.Height.Length);

        var dialogOptionsArea = new Rectangle(
            searchTemplate.TemplateSearchArea.Width.From,
            searchTemplate.TemplateSearchArea.Height.From,
            searchTemplate.TemplateSearchArea.Width.Length,
            searchTemplate.TemplateSearchArea.Height.Length);

        return new DialogDetectionConfig(new []
        {
            dialogIndicationArea,
            dialogIndicationAreaEmpty, 
            speakerNameArea
        }, 
            dialogOptionsArea);
    }

    private CursorConfigBase GetCursorConfig(PresetConfigBase presetConfig, SearchTemplate searchTemplate)
    {
        presetConfig.CursorConfig.InitialPositionX = (int) (presetConfig.CursorConfig.InitialPosition.X * searchTemplate.DialogOptionWidth);
        return presetConfig.CursorConfig;
    }
}
