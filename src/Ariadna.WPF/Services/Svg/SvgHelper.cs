using Serilog;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ariadna;

internal class SvgHelper : ISvgHelper
{
    private readonly ILogger _logger;

    public SvgHelper(ILogger logger)
    {
        _logger = logger;
    }

    public FrameworkElement CreateImageFromSvg(string filePath)
    {
        var icon = new Image()
        {
            Source = CreateImageSourceFromSvg(filePath)
        };

        return icon;
    }

    public DrawingImage? CreateImageSourceFromSvg(string filePath)
    {
        WpfDrawingSettings settings = new()
        {
            IncludeRuntime = true,
            TextAsGeometry = false,
        };

        // 3. Create a file reader
        FileSvgReader converter = new(settings);

        try
        {
            var drawing = converter.Read(new Uri(filePath));

            if (drawing != null)
            {
                DrawingImage imageSource = new(drawing);

                return imageSource;
            }
        }
        catch (Exception e)
        {
            _logger.Information(filePath);
            _logger.Error(e, "SvgHelper");
        }

        return null;
    }
}