using System;
using System.Windows;
using System.Windows.Input;

namespace Ariadna.Engine
{
    public partial class MapNavigationControl
    {
        #region Private Fields

        private bool _rotate;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty RotateAngleProperty = DependencyProperty.Register(
            nameof(RotateAngle), typeof(double), typeof(MapNavigationControl),
            new PropertyMetadata(default(double), RotateAngleChanged));

        private static void RotateAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MapNavigationControl mapNavigationControl)
                mapNavigationControl.RotateAngleChanged();
        }
        
        public static readonly DependencyProperty NorthDoubleClickCommandProperty = DependencyProperty.Register(
            "NorthDoubleClickCommand", typeof(ICommand), typeof(MapNavigationControl),
            new PropertyMetadata(default(ICommand)));

        private bool _doubleClick;

        #endregion

        #region Public Properties

        public double RotateAngle
        {
            get => (double) GetValue(RotateAngleProperty);
            set => SetValue(RotateAngleProperty, value);
        }
        
        public ICommand? NorthDoubleClickCommand
        {
            get => (ICommand) GetValue(NorthDoubleClickCommandProperty);
            set => SetValue(NorthDoubleClickCommandProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler AngleChanged;

        #endregion

        #region Constructor

        public MapNavigationControl()
        {
            InitializeComponent();

            MouseMove += MapNavigationControl_MouseMove;
            MouseLeftButtonUp += MapNavigationControl_MouseLeftButtonUp;
        }

        #endregion

        #region Private Methods

        private void RotateAngleChanged()
        {
            AngleChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_doubleClick)
            {
                _doubleClick = false;
                return;
            }

            _rotate = true;

            Mouse.Capture(this);
        }

        private void MapNavigationControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_rotate)
            {
                var center = new Point(ActualWidth / 2, ActualHeight / 2);

                var pos = e.GetPosition(this);

                var vector = pos - center;

                RotateAngle = -Vector.AngleBetween(vector, new Vector(0, 1)) + 180;
            }
        }

        private void MapNavigationControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_rotate)
            {
                _rotate = false;

                Mouse.Capture(null);
            }
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MouseMove -= MapNavigationControl_MouseMove;
            MouseLeftButtonUp -= MapNavigationControl_MouseLeftButtonUp;

            if (_rotate)
            {
                _rotate = false;

                Mouse.Capture(null);
            }

            NorthDoubleClickCommand?.Execute(null);

            MouseMove += MapNavigationControl_MouseMove;
            MouseLeftButtonUp += MapNavigationControl_MouseLeftButtonUp;
            _doubleClick = true;
        }

        #endregion
    }
}