namespace AuroraDialogEnhancer.AppConfig.Updater;

internal class UpdateResult
{
    public bool IsChecked { get; set; }
    public bool IsUpdated { get; set; }
    public bool IsSuccess { get; set; }

    public UpdateResult(bool isChecked, bool isUpdated, bool isSuccess)
    {
        IsChecked = isChecked;
        IsUpdated = isUpdated;
        IsSuccess = isSuccess;
    }
}
