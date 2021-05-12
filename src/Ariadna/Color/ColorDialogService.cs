using System.Windows.Forms;
using System.Windows.Media;
using Egorozh.ColorPicker.Dialog;

namespace Ariadna
{
    /// <summary>
    /// Сервис по работе с <see cref="ColorDialog"/>
    /// </summary>
    internal class ColorDialogService : IColorDialogService
    {
        #region Constants

        private const string CustomColorsKey = "CustomColors";

        #endregion

        #region Public Methods

        public Color GetColor(Color color)
        {
            ColorPickerDialog colorPicker = new ()
            {
                Color = color,
            };
            
            var res = colorPicker.ShowDialog();

            if (res != true)
                return color;

            return colorPicker.Color;
        }

        public string GetColor(string color) => GetColor(color.ToColor()).ToString();

        #endregion

        #region Private Methods

        //private static int[] GetCustomColors()
        //{
        //    var customColorsJson = ConfigManager.GetValue(CustomColorsKey, null);

        //    int[] customColors = null;

        //    if (customColorsJson != null)
        //        customColors = JsonConvert.DeserializeObject<int[]>(customColorsJson);

        //    return customColors;
        //}

        //private static void SaveCustomColors(int[] customColors)
        //{
        //    var json = JsonConvert.SerializeObject(customColors);
        //    ConfigManager.SetValue(CustomColorsKey, json);
        //}

        #endregion
    }
}