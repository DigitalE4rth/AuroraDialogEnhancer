﻿using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace WhyOrchid.Controls;

public class PathIcon : IconElement
{
    private Path? _path;

    /// <summary>
    /// Initializes a new instance of the PathIcon class.
    /// </summary>
    public PathIcon()
    {
    }

    static PathIcon()
    {
        ForegroundProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(OnForegroundChanged));
    }

    #region Data
    /// <summary>
    /// Identifies the Data dependency property.
    /// </summary>
    public static readonly DependencyProperty DataProperty = Path.DataProperty.AddOwner(typeof(PathIcon), new FrameworkPropertyMetadata(OnDataChanged));

    /// <summary>
    /// Gets or sets a Geometry that specifies the shape to be drawn. In XAML. this can
    /// also be set using a string that describes Move and draw commands syntax.
    /// </summary>
    /// <returns>A description of the shape to be drawn.</returns>
    public Geometry Data
    {
        get => (Geometry) GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PathIcon)d).ApplyData();
    }
    #endregion

    private protected override void InitializeChildren()
    {
        _path = new Path
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Stretch = Stretch.Uniform
        };

        ApplyForeground();
        ApplyData();

        Children.Add(_path);
    }

    private protected override void OnShouldInheritForegroundFromVisualParentChanged()
    {
        ApplyForeground();
    }

    private protected override void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        if (ShouldInheritForegroundFromVisualParent)
        {
            ApplyForeground();
        }
    }

    private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PathIcon)d).ApplyForeground();
    }

    private void ApplyForeground()
    {
        if (_path is not null)
        {
            _path.Fill = ShouldInheritForegroundFromVisualParent ? VisualParentForeground : Foreground;
        }
    }

    private void ApplyData()
    {
        if (_path is not null)
        {
            _path.Data = Data;
        }
    }
}