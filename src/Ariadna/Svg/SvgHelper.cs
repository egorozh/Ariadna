using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Ariadna
{
    internal class SvgHelper : ISvgHelper
    {
        public FrameworkElement CreateImageFromSvg(string filePath)
        {
            var icon = new Image()
            {
                Source = CreateImageSourceFromSvg(filePath)
            };

            return icon;
        }

        public DrawingImage CreateImageSourceFromSvg(string filePath)
        {
            WpfDrawingSettings settings = new()
            {
                IncludeRuntime = true,
                TextAsGeometry = false,
            };

            // 3. Create a file reader
            FileSvgReader converter = new(settings);

            // 4. Read the SVG file
            var drawing = converter.Read(filePath);

            if (drawing != null)
            {
                DrawingImage imageSource = new(drawing);

                return imageSource;
            }

            return null;
        }
    }
}