using System;
using AuroraDialogEnhancer.Backend.Generics;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public class UpdateFrequencyMapper : IMapper<EUpdateFrequency, TimeSpan>
{
    public TimeSpan Map(EUpdateFrequency obj)
    {
        return obj switch
        {
            EUpdateFrequency.Weekly        => TimeSpan.FromDays(7),
            EUpdateFrequency.EveryTwoWeeks => TimeSpan.FromDays(14),
            EUpdateFrequency.Monthly       => DateTime.Now.AddMonths(1) - DateTime.Now,
            EUpdateFrequency.None          => TimeSpan.Zero,
            _                              => TimeSpan.Zero
        };
    }
}
