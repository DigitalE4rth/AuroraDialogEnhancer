using System.Drawing;
using AuroraDialogEnhancerExtensions.Proxy;
using Extension.GenshinImpact.Presets;
using Extension.GenshinImpact.Templates;

namespace Extension.GenshinImpact.Services;

public class DialogOptionFinderInfoMapper
{
    public DialogOptionFinderProvider Map(Size clientSize)
    {
        var searchTemplate      = new SearchTemplateMapper().Map(clientSize);
        var dialogOptionsFinder = new DialogOptionFinder(searchTemplate);

        var dialogConfig = GetDialogDetectionConfig(searchTemplate);
        var cursorConfig = UpdateCursorConfig(new CursorConfig(), searchTemplate);

        var presetData = new PresetData(dialogConfig, cursorConfig);

        return new DialogOptionFinderProvider(dialogOptionsFinder, presetData);
    }

    private DialogDetectionConfig GetDialogDetectionConfig(PreciseTemplate preciseTemplate)
    {
        var speakerNameArea = new Rectangle(
            preciseTemplate.SpeakerNameArea.Width.From,
            preciseTemplate.SpeakerNameArea.Height.From,
            preciseTemplate.SpeakerNameArea.Width.Length,
            preciseTemplate.SpeakerNameArea.Height.Length);

        var dialogOptionsArea = new Rectangle(
            preciseTemplate.TemplateSearchArea.Width.From,
            preciseTemplate.TemplateSearchArea.Height.From,
            preciseTemplate.TemplateSearchArea.Width.Length,
            preciseTemplate.TemplateSearchArea.Height.Length);

        return new DialogDetectionConfig(new []{ speakerNameArea }, dialogOptionsArea);
    }

    private CursorConfigBase UpdateCursorConfig(CursorConfigBase cursorConfig, PreciseTemplate preciseTemplate)
    {
        cursorConfig.InitialPositionX = (int)(cursorConfig.InitialPosition.X * preciseTemplate.DialogOptionWidth);
        return cursorConfig;
    }
}
