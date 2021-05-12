using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ariadna.Engine.Core;
using Prism.Commands;

namespace Ariadna.Engine
{
    internal class AkimNavigationGrid : Grid, INavigationGrid
    {
        private readonly EngineSettings _settings;
        private readonly MapNavigationControl _navigationControl;

        #region Constructor

        public AkimNavigationGrid(EngineSettings settings)
        {
            _settings = settings;
            Background = Brushes.Transparent;
            ClipToBounds = true;

            _navigationControl = new MapNavigationControl
            {
                RotateAngle = settings.InitAngle,
                NorthDoubleClickCommand = new DelegateCommand(OnNorth)
            };

            Children.Add(_navigationControl);

            _navigationControl.AngleChanged += NavigationControl_AngleChanged;
        }
        
        #endregion

        #region Public Methods

        public void Init()
        {
        }

        public void AddElements(params UIElement[] elements)
        {
            foreach (var uiElement in elements)
                Children.Add(uiElement);
        }

        public void RemoveElements(params UIElement[] elements)
        {
            foreach (var element in elements)
                Children.Remove(element);
        }

        #endregion

        #region Move Map Commands

        private const double Delta = 5;

        private void OnTop()
        {
            _settings.InitDeltaY += Delta;
            //_settings.Latitude += Delta * Math.Cos(_settings.RotateAngle * Math.PI / 180.0);
            //_settings.Longitude -= Delta * Math.Sin(_settings.RotateAngle * Math.PI / 180.0);
        }

        private void OnBottom()
        {
            _settings.InitDeltaY -= Delta;
            //_settings.Latitude -= Delta * Math.Cos(_settings.RotateAngle * Math.PI / 180.0);
            //_settings.Longitude += Delta * Math.Sin(_settings.RotateAngle * Math.PI / 180.0);
        }

        private void OnRight()
        {
            _settings.InitDeltaX -= Delta;
            //_settings.Longitude += Delta * Math.Cos(_settings.RotateAngle * Math.PI / 180.0);
            //_settings.Latitude += Delta * Math.Sin(_settings.RotateAngle * Math.PI / 180.0);
        }

        private void OnLeft()
        {
            _settings.InitDeltaX += Delta;
            //_settings.Longitude -= Delta * Math.Cos(_settings.RotateAngle * Math.PI / 180.0);
            //_settings.Latitude -= Delta * Math.Sin(_settings.RotateAngle * Math.PI / 180.0);
        }

        private void OnNorth()
        {
            _navigationControl.RotateAngle = 0;
        }

        #endregion

        private void NavigationControl_AngleChanged(object sender, EventArgs e)
        {
            _settings.InitAngle = _navigationControl.RotateAngle;
        }

    }
}