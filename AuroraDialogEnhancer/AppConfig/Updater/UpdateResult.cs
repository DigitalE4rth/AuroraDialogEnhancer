namespace AuroraDialogEnhancer.AppConfig.Updater;

internal class UpdateResult
{
    public bool IsChecked         { get; set; }
    public bool IsUpdateAvailable { get; set; }
    public bool IsUpdated         { get; set; }
    public bool IsSuccess         { get; set; }

    public UpdateResult(bool isChecked, bool isUpdateAvailable, bool isUpdated, bool isSuccess)
    {
        IsChecked         = isChecked;
        IsUpdateAvailable = isUpdateAvailable;
        IsUpdated         = isUpdated;
        IsSuccess         = isSuccess;
    }
}
