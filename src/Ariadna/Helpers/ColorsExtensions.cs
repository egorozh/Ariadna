using System.Globalization;
using System.Windows.Media;

namespace Ariadna
{
    public static class ColorsExtensions
    {
        public static Color ToColor(this string color)
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
    }
}