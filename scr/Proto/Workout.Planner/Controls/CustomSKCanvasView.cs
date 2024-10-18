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
        public static readonly BindableProperty GroupChangeProperty = BindableProperty.Create(nameof(GroupChangeProperty), typeof(Dictionary<Muscle, Intensity>), typeof(CustomSKCanvasView), null, BindingMode.OneWay, null, propertyChanged: OnGroupChange);

        private const string IdAttributeName = "id";
        private SKSvg? _svg;
        private Assembly _assembly;
        private Dictionary<string, XElement> _xElementsIds = [];
        private XDocument? _document;

        public CustomSKCanvasView()
        {
            _assembly = GetType().Assembly;
        }

        public Dictionary<Muscle, Intensity> GroupChange
        {
            get { return (Dictionary<Muscle, Intensity>)GetValue(GroupChangeProperty); }
            set { SetValue(GroupChangeProperty, value); }
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

        private static void OnGroupChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomSKCanvasView canvas)
            {
                canvas.ChangeGroupColor((Dictionary<Muscle, Intensity>)newValue);
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

        private void ChangeGroupColor(Dictionary<Muscle, Intensity> keyValuePairs)
        {
            foreach (var item in keyValuePairs)
            {
                if (_xElementsIds.ContainsKey(item.Key.ToString().ToLower()))
                {
                    XElement element = _xElementsIds[item.Key.ToString().ToLower()];
                    element.Attributes().FirstOrDefault()?.ToString();
                    element.Descendants().ToList().ForEach(x => x.Attribute("class") !.Value = "st3");
                }
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
