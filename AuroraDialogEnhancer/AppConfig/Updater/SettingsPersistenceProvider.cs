using System;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public class SettingsPersistenceProvider : IPersistenceProvider
{
    public TimeSpan? GetUpdateFrequency()
    {
        return Properties.Settings.Default.AutoUpdater_Frequency;
    }

    public void SetUpdateFrequency(TimeSpan? frequency)
    {
        frequency ??= TimeSpan.Zero;
        Properties.Settings.Default.AutoUpdater_Frequency = (TimeSpan) frequency;
        Properties.Settings.Default.Save();
    }

    public DateTime? GetLastUpdateTime()
    {
        return Properties.Settings.Default.AutoUpdater_LastUpdateCheckTime == DateTime.MinValue
            ? null
            : Properties.Settings.Default.AutoUpdater_LastUpdateCheckTime;
    }

    public void SetLastUpdateTime(DateTime? lastUpdateTime)
    {
        if (lastUpdateTime is null)
        {
            Properties.Settings.Default.AutoUpdater_LastUpdateCheckTime = DateTime.MinValue;
            Properties.Settings.Default.Save();
            return;
        }

        Properties.Settings.Default.AutoUpdater_LastUpdateCheckTime = (DateTime) lastUpdateTime;
        Properties.Settings.Default.Save();
    }
}
