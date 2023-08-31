using System;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public interface IPersistenceProvider
{
    TimeSpan? GetUpdateFrequency();

    void SetUpdateFrequency(TimeSpan? frequency);

    DateTime? GetLastUpdateTime();
    
    void SetLastUpdateTime(DateTime? lastUpdateTime);
}
