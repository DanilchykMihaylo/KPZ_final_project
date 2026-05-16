using System.Windows;
using System.Windows.Media.Animation;

namespace Checkers.Converters
{
    public static class AnimationHelper
    {
        public static void AnimateFadeIn(UIElement element, double durationMs = 300)
        {
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            element.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}