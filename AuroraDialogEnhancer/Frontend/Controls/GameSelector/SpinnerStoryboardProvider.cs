using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace AuroraDialogEnhancer.Frontend.Controls.GameSelector;

internal class SpinnerStoryboardProvider
{
    public void SetStoryboard(Storyboard storyboard, DependencyObject dependencyObject)
    {
        var spinnerAnimation = new DoubleAnimation(0.0, 360.0, new Duration(TimeSpan.FromMilliseconds(3000)));
        storyboard.Children.Add(spinnerAnimation);
        storyboard.RepeatBehavior = RepeatBehavior.Forever;
        Storyboard.SetTarget(spinnerAnimation, dependencyObject);
        Storyboard.SetTargetProperty(spinnerAnimation, new PropertyPath("RenderTransform.Angle"));
    }
}
