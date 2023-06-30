using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AuroraDialogEnhancer.Frontend.Controls.GameSelector;

public partial class GameSelectorContent : UserControl
{
    private bool _isSpinnerAnimationRunning;
    private readonly Storyboard _spinnerStoryboard;

    public GameSelectorContent()
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
