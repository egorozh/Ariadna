using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Ariadna.Core;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    internal class HelpPanelViewModel : BaseViewModel
    {
        private readonly ICoordinateSystem _coordinateSystem;

        #region Public Properties

        public EngineSettings Settings { get; }

        public string X { get; set; }

        public string Y { get; set; }

        public string Resolution { get; set; }

        public bool Visibility { get; set; }

        #endregion

        public HelpPanelViewModel(EngineSettings settings, ICoordinateSystem coordinateSystem)
        {
            _coordinateSystem = coordinateSystem;
            Settings = settings;

            Visibility = settings.IsShowHelpPanel;
            
            ShowResolution((double) settings.InitResolution);

            coordinateSystem.PropertyChanged += CoordinateSystem_PropertyChanged;
        }

        #region Public Methods

        public void ShowMousePosition(Point mousePos)
        {
            X = mousePos.X.ToString("##.###") + "   ";
            Y = mousePos.Y.ToString("##.###");
        }

        public void ShowResolution(double resolution)
        {
            Resolution = resolution.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        private void CoordinateSystem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ICoordinateSystem.MouseGlobalPosition):
                    ShowMousePosition(_coordinateSystem.MouseGlobalPosition);
                    break;
                case nameof(ICoordinateSystem.Resolution):
                    ShowResolution((double) _coordinateSystem.Resolution);
                    break;
            }
        }
    }
}