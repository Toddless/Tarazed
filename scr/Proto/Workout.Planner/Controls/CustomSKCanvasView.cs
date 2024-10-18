namespace Workout.Planner.Controls
{
    using System.Reflection;
    using System.Xml.Linq;
    using DataModel;
    using SkiaSharp;
    using SkiaSharp.Views.Maui;
    using SkiaSharp.Views.Maui.Controls;
    using Svg.Skia;

    public class CustomSKCanvasView : SKCanvasView
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(SourceProperty), typeof(string), typeof(CustomSKCanvasView), null, BindingMode.OneWay, null, propertyChanged: OnSourcePropertyChanged);
        public static readonly BindableProperty MuscleGroupChangeProperty = BindableProperty.Create(nameof(MuscleGroupChangeProperty), typeof(string), typeof(CustomSKCanvasView), null, BindingMode.OneWay, null, propertyChanged: OnMuscleGroupChange);
        public static readonly BindableProperty IntensityGroupChangeProperty = BindableProperty.Create(nameof(IntensityGroupChangeProperty), typeof(string), typeof(CustomSKCanvasView), null, BindingMode.OneWay, null, propertyChanged: OnIntensityGroupChange);

        private const string IdAttributeName = "id";
        private SKSvg? _svg;
        private Assembly _assembly;
        private Dictionary<string, XElement> _xElementsIds = [];
        private XDocument? _document;

        public CustomSKCanvasView()
        {
            _assembly = GetType().Assembly;
        }

        public string MuscleGroupChange
        {
            get { return (string)GetValue(MuscleGroupChangeProperty); }
            set { SetValue(MuscleGroupChangeProperty, value); }
        }

        public string IntensityGroupChange
        {
            get { return (string)GetValue(IntensityGroupChangeProperty); }
            set { SetValue(IntensityGroupChangeProperty, value); }
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            var info = e.Info;
            var drawRect = new SKRect(0, 0, info.Width, info.Height);

            using (_svg = new SKSvg())
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (_document == null)
                    {
                        return;
                    }

                    _document.Save(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    _svg.Load(memoryStream);
                }

                var original = _svg.Picture!.CullRect;

                var scaleX = info.Width / original.Width;
                var scaleY = info.Height / original.Height;
                var matrix = SKMatrix.CreateScale(scaleX, scaleY);
                canvas.DrawRect(drawRect, new SKPaint() { Color = new SKColor(51, 51, 51) });
                canvas.DrawPicture(_svg.Picture, ref matrix);
            }
        }

        private static void OnIntensityGroupChange(BindableObject bindable, object oldValue, object newValue)
        {

        }

        private static void OnMuscleGroupChange(BindableObject bindable, object oldValue, object newValue)
        {

        }

        private static void OnGroupChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomSKCanvasView canvas)
            {
                canvas.ChangeGroupColor();
                canvas.InvalidateSurface();
            }
        }

        private static void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomSKCanvasView canvas)
            {
                canvas.LoadXmlDocument();
                canvas.InvalidateSurface();
            }
        }

        private void LoadXmlDocument()
        {
            _document = null;
            _xElementsIds = [];
            if (string.IsNullOrWhiteSpace(Source))
            {
                return;
            }

            using (var stream = _assembly.GetManifestResourceStream($"Workout.Planner.Resources.Images.{Source}"))
            {
                if ((stream?.Length ?? 0) == 0)
                {
                    return;
                }

                try
                {
                    _document = XDocument.Load(stream!);
                    _xElementsIds = GetElementIds(_document);
                }
                catch (Exception)
                {
                    _document = null;
                }
            }
        }

        private void ChangeGroupColor(Muscle muscle, Intensity intensity)
        {
            var muscleGroup = muscle.ToString().ToLower();
            var intensityGroup = intensity.ToString().ToLower();
            if (_xElementsIds.ContainsKey(muscleGroup))
            {
                XElement element = _xElementsIds[muscleGroup];
                element.Attributes().FirstOrDefault()?.ToString();
                element.Descendants().ToList().ForEach(x => x.Attribute("class")!.Value = "st3");
            }
        }

        private Dictionary<string, XElement> GetElementIds(XDocument document)
        {
            if (document?.Root == null)
            {
                return [];
            }

            XElement root = document.Root;
            XNamespace docNamespace = root.Name.Namespace;

            return root.Descendants().
                Where(x => !string.IsNullOrWhiteSpace(x.Attribute(IdAttributeName)?.Value?.Trim())).
                ToDictionary(x => x.Attribute(IdAttributeName)!.Value.Trim(), y => y);
        }
    }
}
