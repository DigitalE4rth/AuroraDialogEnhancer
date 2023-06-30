using System.Windows.Media.Animation;
using AuroraDialogEnhancer.Frontend.Controls.GameSelector;

namespace AuroraDialogEnhancer.AppConfig.NotifyIcon;

public partial class NotifyGameContent
{
    private bool _isSpinnerAnimationRunning;
    private readonly Storyboard _spinnerStoryboard;

    public NotifyGameContent()
    {
        InitializeComponent();

        _spinnerStoryboard = new Storyboard();
        new SpinnerStoryboardProvider().SetStoryboard(_spinnerStoryboard, Icon);
    }

    public void BeginAnimation()
    {
        if (_isSpinnerAnimationRunning) return;
        _isSpinnerAnimationRunning = true;
        _spinnerStoryboard.Begin();
    }

    public void StopAnimation()
    {
        if (!_isSpinnerAnimationRunning) return;
        _isSpinnerAnimationRunning = false;
        _spinnerStoryboard.Stop();
    }
}
