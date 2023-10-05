using System;
using System.Collections.Generic;
using System.Linq;
using AuroraDialogEnhancer.Backend.Generics;
using AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ClickablePoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Mappers;

public class KeyBindingViewModelMapper : IMapper<(KeyBindingProfile, List<ClickablePointVmDto>), KeyBindingProfileViewModel>
{
    private readonly KeyInterpreterService _keyInterpreterService;

    public KeyBindingViewModelMapper(KeyInterpreterService keyInterpreterService)
    {
        _keyInterpreterService = keyInterpreterService;
    }

    public KeyBindingProfileViewModel Map((KeyBindingProfile, List<ClickablePointVmDto>) obj)
    {
        var profile = obj.Item1;
        var clickablePointsVmDto = obj.Item2;

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

            Previous        = Map(profile.Previous),
            Select          = Map(profile.Select),
            Next            = Map(profile.Next),
            ClickablePoints = Map(profile.ClickablePoints, clickablePointsVmDto),

            AutoSkip = new AutoSkipViewModel
            {
                Id               = profile.AutoSkip.Id,
                ActivationKeys   = Map(profile.AutoSkip.ActivationKeys),
                AutoSkipType     = profile.AutoSkip.AutoSkipType,
                SkipKeys         = Map(profile.AutoSkip.SkipKeys),
                Delay            = profile.AutoSkip.Delay,
                DoubleClickDelay = profile.AutoSkip.DoubleClickDelay
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

    private Dictionary<string, ClickablePointVm> Map(IEnumerable<ClickablePoint> clickablePoints, IReadOnlyCollection<ClickablePointVmDto> clickablePointsVm)
    {
        return (from point in clickablePoints
                from vmPoint in clickablePointsVm
                where point.Id.Equals(vmPoint.Id, StringComparison.Ordinal)
                select new ClickablePointVm(
                    point.Id,
                    vmPoint.Name,
                    vmPoint.Description,
                    vmPoint.PathIcon,
                    Map(point.Keys)))
            .ToDictionary(vm => vm.Id, vm => vm);
    }
}
