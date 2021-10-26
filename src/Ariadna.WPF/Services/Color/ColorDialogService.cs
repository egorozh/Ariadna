using System.Globalization;
using Egorozh.ColorPicker.Dialog;
using System.Windows.Media;

namespace Ariadna;

internal class ColorDialogService : IColorDialogService
{
    #region Constants

    private const string CustomColorsKey = "CustomColors";

    #endregion

    #region Public Methods

    public Color GetColor(Color color)
    {
        ColorPickerDialog colorPicker = new()
        {
            Color = color,
        };

        var res = colorPicker.ShowDialog();

        if (res != true)
            return color;

        return colorPicker.Color;
    }

    public string GetColor(string color) => GetColor(ToColor(color)).ToString();

    public static Color ToColor(string color)
    {
        var withoutSharpChar = color.Substring(1);

        if (withoutSharpChar.Length == 6)
        {
            var r = byte.Parse(withoutSharpChar.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(withoutSharpChar.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(withoutSharpChar.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromArgb(255, r, g, b);
        }
        else if (withoutSharpChar.Length == 8)
        {
            var a = byte.Parse(withoutSharpChar.Substring(0, 2), NumberStyles.HexNumber);
            var r = byte.Parse(withoutSharpChar.Substring(2, 2), NumberStyles.HexNumber);
            var g = byte.Parse(withoutSharpChar.Substring(4, 2), NumberStyles.HexNumber);
            var b = byte.Parse(withoutSharpChar.Substring(6, 2), NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        return new Color();
    }

    #endregion
}