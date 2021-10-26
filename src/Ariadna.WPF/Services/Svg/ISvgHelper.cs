using System.Windows;
using System.Windows.Media;

namespace Ariadna;

public interface ISvgHelper
{
    FrameworkElement CreateImageFromSvg(string filePath);

    DrawingImage? CreateImageSourceFromSvg(string filePath);
}