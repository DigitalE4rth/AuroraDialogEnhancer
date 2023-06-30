using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Controls;

internal partial class HuePicker : SliderPicker
    {
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
            nameof(Hue), 
            typeof(double), 
            typeof(HuePicker), 
            new PropertyMetadata(0.0, OnHueChanged));

        public HuePicker()
        {
            InitializeComponent();
        }

        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        private static void OnHueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var huePicker = (HuePicker)o;
            huePicker.UpdateAdorner((double)e.NewValue);
        }

        private void UpdateAdorner(double hue)
        {
            double percent = hue / 360;

            // Make it so that the arrow doesn't jump back to the top when it goes to the bottom
            Point mousePos = Mouse.GetPosition(this);
            if (percent == 0 && ActualHeight - mousePos.Y < 1)
            {
                percent = 1;
            }

            AdornerVerticalPercent = percent;
        }

        protected override void OnAdornerPositionChanged(double verticalPercent)
        {
            var c = GetColorAtOffset(HueGradients.GradientStops, verticalPercent);
            Hue = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetHue();
        }

        /// <summary>
        /// Gets the color of a gradient stop collection with the given index
        /// </summary>
        /// <param name="collection">Colletion of colors</param>
        /// <param name="offset">The offset</param>
        /// <returns>The color at the offset</returns>
        private Color GetColorAtOffset(GradientStopCollection collection, double offset)
        {
            GradientStop[] stops = collection.OrderBy(x => x.Offset).ToArray();
            switch (offset)
            {
                case <= 0:
                    return stops[0].Color;
                case >= 1:
                    return stops[stops.Length - 1].Color;
            }

            GradientStop left = stops[0];
            GradientStop? right = null;

            foreach (GradientStop stop in stops)
            {
                if (stop.Offset >= offset)
                {
                    right = stop;
                    break;
                }

                left = stop;
            }

            double percent = Math.Round((offset - left.Offset) / (right!.Offset - left.Offset), 3);
            byte a = (byte)((right.Color.A - left.Color.A) * percent + left.Color.A);
            byte r = (byte)((right.Color.R - left.Color.R) * percent + left.Color.R);
            byte g = (byte)((right.Color.G - left.Color.G) * percent + left.Color.G);
            byte b = (byte)((right.Color.B - left.Color.B) * percent + left.Color.B);
            return Color.FromArgb(a, r, g, b);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            AdjustRenderPosition();
        }
    }
