using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Класс, реализующий режим выделения фигур
    /// </summary>
    internal sealed class SelectHelper2D : ISelectHelper2D
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;

        /// <summary>
        /// Примитив для области выделения
        /// </summary>
        private Path _selectedRectangle;

        /// <summary>
        /// Кисть для области выделения в режиме частичного захвата
        /// </summary>
        private readonly SolidColorBrush _intersectRectangleFill = Brushes.LightGreen;

        /// <summary>
        /// Кисть для области выделения в режиме полного захвта
        /// </summary>
        private readonly SolidColorBrush _fullInsideRectangleFill = Brushes.LightBlue;

        /// <summary>
        /// Начальная точка массового выделения фигур
        /// - левый верхний угол прямоугольника
        /// </summary>
        private Point _startPoint;

        /// <summary>
        /// Происходит выделение
        /// </summary>
        private bool _isSelecting;

        private bool _isOn;

        #endregion

        #region Constructor

        /// <summary>
        /// Дефолтный конструктор
        /// </summary>
        public SelectHelper2D(IAriadnaEngine ariadnaEngine)
        {
            _ariadnaEngine = ariadnaEngine;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            _selectedRectangle = ShapeFactory.CreateSelectedRectangle();
            _selectedRectangle.Data = new RectangleGeometry();

            _selectedRectangle.AddToCanvas(_ariadnaEngine.Canvas);

            OnSelectedHelper();
        }

        /// <summary>
        /// Включение режима выделения фигур
        /// </summary>
        public void OnSelectedHelper()
        {
            if (_isOn)
                return;

            _isOn = true;

            _ariadnaEngine.Canvas.MouseLeftButtonDown += CanvasMouseLeftButtonDown;
            _ariadnaEngine.Canvas.MouseLeftButtonUp += CanvasMouseLeftButtonUp;
            _ariadnaEngine.Canvas.MouseMove += CanvasMouseMove;

            _ariadnaEngine.CoordinateSystem.OnMoving();

            if (_isSelecting)
            {
                Mouse.Capture(null);
                ((RectangleGeometry) _selectedRectangle.Data).Rect = new Rect();
                _isSelecting = false;
            }
        }

        /// <summary>
        /// Отключение режима выделения фигур
        /// </summary>
        public void OffSelectedHelper()
        {
            if (!_isOn)
                return;

            _isOn = false;

            _ariadnaEngine.Canvas.MouseLeftButtonDown -= CanvasMouseLeftButtonDown;
            _ariadnaEngine.Canvas.MouseLeftButtonUp -= CanvasMouseLeftButtonUp;
            _ariadnaEngine.Canvas.MouseMove -= CanvasMouseMove;

            _ariadnaEngine.CoordinateSystem.OnMoving();

            if (_isSelecting)
            {
                Mouse.Capture(null);
                _isSelecting = false;
                ((RectangleGeometry) _selectedRectangle.Data).Rect = new Rect();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Происходит при щелчке левой кнопки мыши на <see cref="Canvas"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isSelecting && !_isOn)
                return;

            _startPoint = e.GetPosition(_ariadnaEngine.Canvas);

            _isSelecting = true;
            Mouse.Capture(_ariadnaEngine.Canvas);
            _ariadnaEngine.CoordinateSystem.OffMoving();
        }

        /// <summary>
        /// Происходит при перемещении указателя мыши над <see cref="Canvas"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_isSelecting && _isOn)
            {
                var mousePos = e.GetPosition(_ariadnaEngine.Canvas);

                // Рисуем прямоугольник
                ((RectangleGeometry) _selectedRectangle.Data).Rect = new Rect(_startPoint, mousePos);

                // Устанавливаем цвет прямоугольника в зависимости от режима выделения:
                _selectedRectangle.Fill = mousePos.X > _startPoint.X
                    ? _fullInsideRectangleFill
                    : _intersectRectangleFill;
            }
        }

        /// <summary>
        /// Происходит при отпускании левой кнопки мыши над <see cref="Canvas"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isSelecting && !_isOn) return;

            var cursorPos = e.GetPosition(_ariadnaEngine.Canvas);

            List<ISelectedFigure2D> unselected = null;

            // Одиночное выделение:
            if (_startPoint == cursorPos)
            {
                if (!InverseSelectionCondition())
                {
                    unselected = new List<ISelectedFigure2D>();

                    // Unselected figures:
                    foreach (var figure in _ariadnaEngine.Figures)
                        if (figure is ISelectedFigure2D selectedFigure2D)
                            unselected.Add(selectedFigure2D);
                }

                Geometry ellipseGeometry = new EllipseGeometry(_ariadnaEngine.CoordinateSystem.GetGlobalPoint(cursorPos),
                    _ariadnaEngine.CoordinateSystem.GetGlobalLength(_ariadnaEngine.Settings.SelectionPointRadius),
                    _ariadnaEngine.CoordinateSystem.GetGlobalLength(_ariadnaEngine.Settings.SelectionPointRadius));

                var selectedFigure = GlobalFigureHitTester.SingleHitTest(_ariadnaEngine.Figures, ellipseGeometry);

                if (selectedFigure != null)
                {
                    var list = new List<ISelectedFigure2D>() {selectedFigure};

                    if (selectedFigure.IsSelected)
                    {
                        ((FigureCollection) _ariadnaEngine.Figures).GroupSelect(null, unselected ?? list);
                    }
                    else
                    {
                        if (unselected != null && unselected.Contains(selectedFigure))
                            unselected.Remove(selectedFigure);

                        ((FigureCollection) _ariadnaEngine.Figures).GroupSelect(list, unselected);
                    }
                }
                else
                {
                    _ariadnaEngine.Figures.UnselectAllFigures();
                }
            }

            // Групповое выделение: 
            else
            {
                if (!AddedGroupSelectionCondition() && !InverseSelectionCondition())
                {
                    unselected = new List<ISelectedFigure2D>();

                    // Unselected figures:
                    foreach (var figure in _ariadnaEngine.Figures)
                        if (figure is ISelectedFigure2D selectedFigure2D)
                            unselected.Add(selectedFigure2D);
                }

                var intersectSelectionMode = cursorPos.X > _startPoint.X
                    ? IntersectSelectionMode.FullInside
                    : IntersectSelectionMode.Intersect;


                var rect = ((RectangleGeometry) _selectedRectangle.Data).Rect;

                Geometry rectangleGeometry =
                    PathGeometry.CreateFromGeometry(new RectangleGeometry(new Rect(rect.BottomLeft, rect.TopRight)));
                
                var (selectedFigures, unSelectedFigures) =
                    GlobalFigureHitTester.HitTest(_ariadnaEngine.Figures, InverseSelectionCondition(),
                        _ariadnaEngine.CoordinateSystem.GetGlobalGeometry(rectangleGeometry),
                        intersectSelectionMode, unselected);

                ((FigureCollection) _ariadnaEngine.Figures).GroupSelect(selectedFigures, unSelectedFigures);
            }

            Mouse.Capture(null);
            _ariadnaEngine.CoordinateSystem.OnMoving();
            _isSelecting = false;

            ((RectangleGeometry) _selectedRectangle.Data).Rect = new Rect();
        }

        /// <summary>
        /// Условие группового выбора c добавлением к выделенным объектам
        /// </summary>
        /// <returns></returns>
        private static bool AddedGroupSelectionCondition() => Keyboard.IsKeyDown(Key.LeftShift);

        /// <summary>
        /// Условие выбора нескольких элементов по очередно
        /// </summary>
        /// <returns></returns>
        private static bool InverseSelectionCondition() => Keyboard.IsKeyDown(Key.LeftCtrl);

        #endregion
    }
}