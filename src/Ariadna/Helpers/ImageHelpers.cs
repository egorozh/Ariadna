using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ariadna
{
    public static class ImageHelpers
    {
        public static string GenerateImageFilter()
        {
            var filter = new StringBuilder();

            var extensions = new List<(string, string)>();
            var codecs = ImageCodecInfo.GetImageEncoders();

            foreach (var c in codecs)
            {
                var codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                extensions.Add((codecName, c.FilenameExtension));
            }

            extensions.Add(("ICO Files", "*.ICO"));
            extensions.Add(("SVG Files", "*.SVG"));

            const string sep = "|";

            var (name, ext) = GetAllSupportedFilesFilter(extensions);

            filter.Append($"{name} ({ext})|{ext}");

            foreach (var (name1, ext1) in extensions) 
                filter.Append($"{sep}{name1} ({ext1})|{ext1}");
            
            return filter.ToString();
        }

        private static (string, string) GetAllSupportedFilesFilter(List<(string, string)> extensions)
        {
            var ex = new StringBuilder();

            const string sep = ";";

            for (var i = 0; i < extensions.Count; i++)
            {
                ex.Append(extensions[i].Item2);

                if (i < extensions.Count - 1)
                    ex.Append(sep);
            }

            return ("All Supported Files", ex.ToString());
        }

        public static ImageSource CreateImageSource(string imagePath, ISvgHelper svgHelper)
        {
            if (imagePath.ToUpper().EndsWith(".SVG"))
                return svgHelper.CreateImageSourceFromSvg(imagePath);
            else
                return new BitmapImage(new Uri(imagePath));
        }
    }
}