using System;
using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Mappers;

public class KeyBindingViewModelMapper : IMapper<(KeyBindingProfile, List<InteractionPointVmDto>), KeyBindingProfileViewModel>
{
    private readonly KeyInterpreterService _keyInterpreterService;

    public KeyBindingViewModelMapper(KeyInterpreterService keyInterpreterService)
    {
        _keyInterpreterService = keyInterpreterService;
    }

    public KeyBindingProfileViewModel Map((KeyBindingProfile, List<InteractionPointVmDto>) obj)
    {
        var profile = obj.Item1;
        var interactionPointsVmDto = obj.Item2;

        return new KeyBindingProfileViewModel
        {
            IsCursorHideOnManualClick   = profile.IsCursorHideOnManualClick,
            IsCycleThrough              = profile.IsCycleThrough,
            SingleDialogOptionBehaviour = profile.SingleDialogOptionBehaviour,
            NumericActionBehaviour      = profile.NumericActionBehaviour,
            CursorBehaviour             = profile.CursorBehaviour,

            Reload      = Map(profile.Reload),
            PauseResume = Map(profile.PauseResume),
            Screenshot  = Map(profile.Screenshot),
            HideCursor  = Map(profile.HideCursor),

            Previous = Map(profile.Previous),
            Select   = Map(profile.Select),
            Next     = Map(profile.Next),
            Last     = Map(profile.Last),

            InteractionPoints = Map(profile.InteractionPoints, interactionPointsVmDto),

            AutoSkipConfig = new AutoSkipConfigViewModel
            {
                ActivationKeys    = Map(profile.AutoSkipConfig.ActivationKeys),
                SkipMode          = profile.AutoSkipConfig.SkipMode,
                StartCondition    = profile.AutoSkipConfig.StartCondition,
                SkipKeys          = Map(profile.AutoSkipConfig.SkipKeys),
                ScanDelayRegular  = profile.AutoSkipConfig.ScanDelayRegular,
                ClickDelayRegular = profile.AutoSkipConfig.ClickDelayRegular,
                ScanDelayReply    = profile.AutoSkipConfig.ScanDelayReply,
                ClickDelayReply   = profile.AutoSkipConfig.ClickDelayReply
            },

            One   = Map(profile.One),
            Two   = Map(profile.Two),
            Three = Map(profile.Three),
            Four  = Map(profile.Four),
            Five  = Map(profile.Five),
            Six   = Map(profile.Six),
            Seven = Map(profile.Seven),
            Eight = Map(profile.Eight),
            Nine  = Map(profile.Nine),
            Ten   = Map(profile.Ten)
        };
    }

    private ActionViewModel Map(IEnumerable<List<GenericKey>> lowActions)
    {
        return new ActionViewModel(lowActions.Select(rawKeys => new TriggerViewModel(rawKeys, _keyInterpreterService.InterpretKeys(rawKeys))).ToList());
    }

    private TriggerViewModel Map(List<GenericKey> lowActions)
    {
        return new TriggerViewModel(lowActions, _keyInterpreterService.InterpretKeys(lowActions));
    }

    private Dictionary<string, InteractionPointVm> Map(IEnumerable<InteractionPoint> interactionPoints, IReadOnlyCollection<InteractionPointVmDto> interactionPointsVm)
    {
        return (from point in interactionPoints
                from vmPoint in interactionPointsVm
                where point.Id.Equals(vmPoint.Id, StringComparison.Ordinal)
                select new InteractionPointVm(
                    point.Id,
                    vmPoint.Name,
                    vmPoint.Description,
                    vmPoint.PathIcon,
                    Map(point.ActivationKeys)))
            .ToDictionary(vm => vm.Id, vm => vm);
    }
}
