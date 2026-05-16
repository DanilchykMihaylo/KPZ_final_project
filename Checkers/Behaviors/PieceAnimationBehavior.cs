using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Checkers.Behaviors
{
    public static class PieceAnimationBehavior
    {
        public static readonly DependencyProperty AnimatedXProperty =
            DependencyProperty.RegisterAttached(
                "AnimatedX",
                typeof(double),
                typeof(PieceAnimationBehavior),
                new PropertyMetadata(0.0, OnAnimatedXChanged));

        public static readonly DependencyProperty AnimatedYProperty =
            DependencyProperty.RegisterAttached(
                "AnimatedY",
                typeof(double),
                typeof(PieceAnimationBehavior),
                new PropertyMetadata(0.0, OnAnimatedYChanged));

        public static double GetAnimatedX(DependencyObject obj) =>
            (double)obj.GetValue(AnimatedXProperty);

        public static void SetAnimatedX(DependencyObject obj, double value) =>
            obj.SetValue(AnimatedXProperty, value);

        public static double GetAnimatedY(DependencyObject obj) =>
            (double)obj.GetValue(AnimatedYProperty);

        public static void SetAnimatedY(DependencyObject obj, double value) =>
            obj.SetValue(AnimatedYProperty, value);

        private static void OnAnimatedXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
                AnimatePosition(element, Canvas.LeftProperty, (double)e.NewValue);
        }

        private static void OnAnimatedYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
                AnimatePosition(element, Canvas.TopProperty, (double)e.NewValue);
        }

        private static void AnimatePosition(UIElement element, DependencyProperty property, double to)
        {
            var rawValue = element.GetValue(property);
            var current = rawValue is double d && !double.IsNaN(d) ? d : to;

            if (Math.Abs(current - to) < 0.1)
            {
                element.SetValue(property, to);
                return;
            }

            var animation = new DoubleAnimation
            {
                From = current,
                To = to,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            element.BeginAnimation(property, animation);
        }
    }
}