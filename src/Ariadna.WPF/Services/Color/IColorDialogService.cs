using System.Windows.Media;

namespace Ariadna;

public interface IColorDialogService
{
    Color GetColor(Color color);

    string GetColor(string color);
}