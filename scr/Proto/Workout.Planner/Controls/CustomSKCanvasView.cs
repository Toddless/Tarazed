namespace Workout.Planner.Controls
{
    using System.Collections;
    using System.Reflection;
    using System.Xml.Linq;
    using DataModel;
    using SkiaSharp;
    using SkiaSharp.Views.Maui;
    using SkiaSharp.Views.Maui.Controls;
    using Svg.Skia;
    using Workout.Planner.Helper;
    using Workout.Planner.Models;

    public class CustomSKCanvasView : SKCanvasView
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(SourceProperty), typeof(string), typeof(CustomSKCanvasView), null, BindingMode.OneWay, null, propertyChanged: OnSourcePropertyChanged);
        public static readonly BindableProperty MuscleGroupChangeProperty = BindableProperty.Create(nameof(MuscleGroupChangeProperty), typeof(IEnumerable), typeof(CustomSKCanvasView), null, BindingMode.OneWay, null, propertyChanged: OnMuscleGroupChange);

        private const string IdAttributeName = "id";
        private Dictionary<string, XElement> _changedXElementsIds = [];
        private Dictionary<string, XElement> _xElementsIds = [];
        private XDocument? _document;
        private Assembly _assembly;
        private SKSvg? _svg;

        public CustomSKCanvasView()
        {
            _assembly = GetType().Assembly;
        }

        public IEnumerable MuscleGroupChange
        {
            get { return (IEnumerable)GetValue(MuscleGroupChangeProperty); }
            set { SetValue(MuscleGroupChangeProperty, value); }
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

            using (_svg = new SKSvg())
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (_document == null)
                    {
                        _document = new();
                    }

                    _document.Save(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    _svg.Load(memoryStream);
                }

                var original = _svg.Picture!.CullRect;

                var scaleX = info.Width / original.Width;
                var scaleY = info.Height / original.Height;
                var matrix = SKMatrix.CreateScale(scaleX, scaleY);
                canvas.DrawPicture(_svg.Picture, ref matrix);
            }
        }

        private static void OnMuscleGroupChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomSKCanvasView canvas)
            {
                canvas.SetColorToDefault();
                canvas.ChangeGroupColor(newValue);
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

        private void ChangeGroupColor(object newValue)
        {
            if (newValue is IEnumerable<MuscleIntensityLevelModel> s)
            {
                foreach (var item in s)
                {
                    XElement element = _xElementsIds[item.Muscle.ToString().ToLower()];
                    _changedXElementsIds.Add(item.Muscle.ToString().ToLower(), element);
                    element.Descendants().ToList().ForEach(x => x.Attribute("class")!.Value = IntensityHelper.GetIntensity(item.Intensity));
                }
            }
        }

        private void SetColorToDefault()
        {
            if(_changedXElementsIds.Count == 0)
            {
                return;
            }

            foreach (var item in _changedXElementsIds)
            {
                item.Value.Descendants().ToList().ForEach(x => x.Attribute("class")!.Value = IntensityHelper.GetIntensity(Intensity.None));
            }

            _changedXElementsIds.Clear();
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
