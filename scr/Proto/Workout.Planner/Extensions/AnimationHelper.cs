namespace Workout.Planner.Extensions
{
    public static class AnimationHelper
    {
        public static void PoppingIn(ContentPage page)
        {
            var contentSize = page.Content.Measure(page.Window.Width, page.Window.Height, MeasureFlags.IncludeMargins);
            var contentHeight = contentSize.Request.Height;

            page.Content.TranslationY = contentHeight;

            page.Animate(
                "Background",
                callback: v => page.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0d,
                end: 0.7d,
                rate: 32,
                length: 350,
                easing: Easing.CubicInOut,
                finished: (v, k) => page.Background = new SolidColorBrush(Colors.Black.WithAlpha(0.7f)));

            page.Animate(
                "Content",
                callback: v => page.Content.TranslationY = (int)(contentHeight - v),
                start: 0,
                end: contentHeight,
                length: 500,
                easing: Easing.CubicInOut,
                finished: (v, k) => page.Content.TranslationY = 0);
        }

        public static Task PoppingOut(ContentPage page)
        {
            var done = new TaskCompletionSource();

            var contentSize = page.Content.Measure(page.Window.Width, page.Window.Height, MeasureFlags.IncludeMargins);
            var windowHeight = contentSize.Request.Height;
            page.Animate(
                "Background",
                callback: v => page.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0.7d,
                end: 0d,
                rate: 32,
                length: 350,
                easing: Easing.CubicIn,
                finished: (v, k) => page.Background = new SolidColorBrush(Colors.Black.WithAlpha(0.0f)));

            page.Animate(
                "Content",
                callback: v => page.Content.TranslationY = (int)(windowHeight - v),
                start: windowHeight,
                end: 0,
                length: 500,
                easing: Easing.CubicInOut,
                finished: (v, k) =>
                {
                    page.Content.TranslationY = windowHeight;
                    done.TrySetResult();
                });

            return done.Task;
        }
    }
}
