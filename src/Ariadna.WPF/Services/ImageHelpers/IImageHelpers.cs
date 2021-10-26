using System.Windows.Media;

namespace Ariadna;

public interface IImageHelpers
{
    string GenerateImageFilter();

    ImageSource CreateImageSource(string imagePath);
}