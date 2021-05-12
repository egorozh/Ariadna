using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Ariadna.Core;

namespace Ariadna
{
    public class MagicOptions : BaseViewModel, IMagicOptions
    {
        #region Private Fields

        private string _fileName;

        #endregion

        #region Public Properties

        public ThemeOptions Theme { get; set; } = new ThemeOptions();

        public bool IsShowRibbon { get; set; }

        #endregion

        #region Constructor

        public MagicOptions()
        {
        }

        public MagicOptions(string basePath, string configFileName)
        {
            var options = new MagicOptions();

            try
            {
                _fileName = Path.Combine(basePath, configFileName);

                if (!File.Exists(_fileName))
                    File.Create(_fileName);

                options = JsonSerializer.Deserialize<MagicOptions>(File.ReadAllText(_fileName));
            }
            catch (Exception e)
            {
            }

            Theme = options.Theme ?? new ();
            IsShowRibbon = options.IsShowRibbon;


            Theme.PropertyChanged += Theme_PropertyChanged;
            PropertyChanged += Theme_PropertyChanged;
        }

        #endregion

        #region Private Methods

        private void Theme_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var json = JsonSerializer.Serialize(this);

            File.WriteAllText(_fileName, json);
        }

        #endregion
    }
}