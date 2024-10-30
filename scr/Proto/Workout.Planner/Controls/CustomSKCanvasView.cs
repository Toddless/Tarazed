namespace Workout.Planner.Controls
{
    using System.Collections;
    using System.Linq;
    using System.Xml.Linq;
    using DataModel;
    using Microsoft.Maui.Controls;
    using SkiaSharp;
    using SkiaSharp.Views.Maui;
    using SkiaSharp.Views.Maui.Controls;
    using Svg.Skia;
    using Workout.Planner.Helper;
    using Workout.Planner.Models;

    public class CustomSKCanvasView : SKCanvasView
    {
        /// <summary>
        /// Der Pfad zur SVG-Datei. Der Name der Datei soll
        /// kleingeschrieben und im SVG-Format (.svg) sein.
        /// </summary>
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            nameof(SourceProperty),
            typeof(string),
            typeof(CustomSKCanvasView),
            null,
            BindingMode.OneWay,
            null,
            propertyChanged: OnSourcePropertyChanged);

        /// <summary>
        /// Reagiert auf Änderungen in einer verknüpften Kollektion.
        /// </summary>
        public static readonly BindableProperty MuscleGroupChangeProperty = BindableProperty.Create(
            nameof(MuscleGroupChangeProperty),
            typeof(IEnumerable),
            typeof(CustomSKCanvasView),
            null,
            BindingMode.OneWay,
            null,
            propertyChanged: OnMuscleGroupChange);

        private const string IdAttributeName = "id";

        /// <summary>
        /// Geänderte Elemente, die beim aufruf <seealso cref="OnMuscleGroupChange(BindableObject, object, object)"/> zurückgesetzt werden.
        /// </summary>
        private Dictionary<string, XElement> _changedXElementsIds = new();

        /// <summary>
        /// Hier sind alle Elemente und alle Kinder des Elements gespeichert.
        /// </summary>
        private Dictionary<string, XElement> _xElementsIds = new();

        /// <summary>
        /// Das Dokument wird beim Aufruf <seealso cref="OnPaintSurface(SKPaintSurfaceEventArgs)"/> benutzt.
        /// </summary>
        private XDocument? _document;

        public CustomSKCanvasView()
        {
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

        /// <summary>
        /// Zeichnet das Bild auf den Bildschirm.
        /// Die Bildparameter werden dem Dokument entnommen.
        /// </summary>
        /// <param name="eventArgs">To be added.</param>
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs eventArgs)
        {
            base.OnPaintSurface(eventArgs);
            var canvas = eventArgs.Surface.Canvas;
            canvas.Clear();
            var info = eventArgs.Info;

            using (var svg = new SKSvg())
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (_document == null)
                    {
                        _document = new();
                    }

                    _document.Save(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    svg.Load(memoryStream);
                }

                var original = svg.Picture!.CullRect;

                var scaleX = info.Width / original.Width;
                var scaleY = info.Height / original.Height;
                var matrix = SKMatrix.CreateScale(scaleX, scaleY);
                canvas.DrawPicture(svg.Picture, ref matrix);
            }
        }

        /// <summary>
        /// Wird bei Änderungen in der Kollektion aufgerufen.
        /// Ändert die Farben und zeichnet das Bild neu.
        /// </summary>
        private static void OnMuscleGroupChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomSKCanvasView canvas)
            {
                canvas.SetColorToDefault();
                canvas.ChangeGroupColor(newValue);
                canvas.InvalidateSurface();
            }
        }

        /// <summary>
        /// Wenn sich der Pfad ändert, wird das Dokument neu geladen.
        /// </summary>
        private static void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomSKCanvasView canvas)
            {
                canvas.LoadXmlDocument();
                canvas.InvalidateSurface();
            }
        }

        /// <summary>
        /// Die Methode sucht nach dem angegebenen Pfad <seealso cref="Source"/>
        /// und speichert als Dokument. Gleichzeitig wird die Kollektion <seealso cref="_xElementsIds"/> gefüllt.
        /// </summary>
        private void LoadXmlDocument()
        {
            _document = null;
            _xElementsIds = new();
            if (string.IsNullOrWhiteSpace(Source))
            {
                return;
            }

            using (var stream = GetType().Assembly.GetManifestResourceStream($"Workout.Planner.Resources.Images.{Source}"))
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

        /// <summary>
        /// Ändert die Farbe des Elements, wenn die gespeicherte ID des Elements mit angekommende übereinstimmt.
        /// </summary>
        private void ChangeGroupColor(object newValue)
        {
            if (newValue is IEnumerable<MuscleIntensityLevelModel> collection)
            {
                foreach (var item in collection)
                {
                    if (string.IsNullOrWhiteSpace(item.Muscle))
                    {
                        continue;
                    }

                    XElement element = _xElementsIds[item.Muscle];
                    element.Descendants()
                        .ToList()
                        .ForEach(x => x.Attribute("class") !.Value = IntensityHelper.GetIntensity(item.Intensity));
                    _changedXElementsIds.Add(item.Muscle, element);
                }
            }
        }

        /// <summary>
        /// Setzt die geänderte Werte zurück.
        /// </summary>
        private void SetColorToDefault()
        {
            foreach (var item in _changedXElementsIds)
            {
                item.Value.Descendants()
                    .ToList()
                    .ForEach(x => x.Attribute("class") !.Value = IntensityHelper.GetIntensity(Intensity.None));
            }

            _changedXElementsIds.Clear();
        }

        /// <summary>
        /// Speichert alle Elemente mit dem Attribut "id" in einer Kollektion.
        /// </summary>
        private Dictionary<string, XElement> GetElementIds(XDocument document)
        {
            if (document?.Root == null)
            {
                return new();
            }

            XElement root = document.Root;
            XNamespace docNamespace = root.Name.Namespace;

            return root.Descendants().
               Where(x => !string.IsNullOrWhiteSpace(x.Attribute(IdAttributeName)?.Value?.Trim())).
               ToDictionary(key => key.Attribute(IdAttributeName) !.Value.Trim(), value => value);
        }
    }
}
