using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AKIM.Maths;
using Ariadna.Engine.Core;
using Geometry = System.Windows.Media.Geometry;
using LineSegment = System.Windows.Media.LineSegment;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal sealed class RulerWorkspace : IRulerWorkspace
    {
        #region Private Fields

        /// <summary>
        /// Сегменты, составляющие геометрию
        /// </summary>
        private readonly ObservableCollection<SegmentFigure> _segments = new ObservableCollection<SegmentFigure>();

        private PreviewSegment _previewSegment;

        /// <summary>
        /// Примитив указателя мыши, отображаемый либо в точках привязки либо в координатах указателя мыши
        /// в зависимости от включенных режимов примагничивания
        /// </summary>
        private Rectangle _mousePoint;

        private bool _middleArcPointSetted;
        private Point _middleArcPoint;
        private Point _magnetPointWithoutShiftMode;
        private Point _magnetPointShiftMode;
        private bool _startPointSetted;

        private Figure _selectedFigure;

        private bool _lineSplitted;
        private bool _lineConverted;

        #region Fill & Closed

        private Path _filledPath;
        private ClosedSegmentFigure _closedSegment;

        #endregion

        #region Move

        private Figure _movedFigure;
        private Point _movedPointOldValue;
        private Point _movedSegmentCenterOldValue;
        private Vector _movedOffsetVector;
        private Line _helpMovingLine;

        #endregion

        #endregion

        #region Internal Fields

        internal readonly RulerCreator RulerCreator;
        internal PointFigure StartPoint;

        #endregion

        #region Public Properties

        public bool IsClosed { get; private set; }

        #endregion

        #region Events

        public event EventHandler SelectedChanged;

        #endregion

        #region Constructor

        public RulerWorkspace(RulerCreator rulerCreator)
        {
            RulerCreator = rulerCreator;

            Init();

            if (RulerCreator.Figure != null)
                EditingAction();
            else
            {
                IsClosed = RulerCreator.IsClosed;

                ShowMessage(HelpMessages.SetStartPointMessage);
            }
        }

        #endregion

        #region Public Methods

        public bool CanContinue()
        {
            if (_startPointSetted)
            {
                if (StartPoint.IsSelected)
                    return true;

                var lastSegment = _segments.LastOrDefault();

                if (lastSegment != null)
                {
                    if (lastSegment.PointFigure.IsSelected)
                        return true;
                }
            }

            return false;
        }

        public void Continue()
        {
            _middleArcPointSetted = false;

            CreatePreviewSegment();
            UpdateFilledAndClosed();
        }

        public void SetCreationMode(CreationMode mode)
        {
            Continue();

            UpdateHelpPoints();
        }

        public void SetIsClosed(bool isClosed)
        {
            IsClosed = isClosed;
            UpdateFilledAndClosed();
            _movedFigure = null;
        }

        public bool CanDeleteSelectedPoint()
        {
            if (_selectedFigure is PointFigure && !RulerCreator.IsContinue)
            {
                if (!IsClosed && _segments.Count >= 2)
                {
                    return true;
                }

                if (IsClosed && _segments.Count >= 3)
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteSelectedPoint()
        {
            if (!CanDeleteSelectedPoint())
                return;

            var segment = SearchSegment((PointFigure) _selectedFigure, out var nextSegment, out var prevSegment);

            Actions.DeleteNode(this, GetIndex((PointFigure) _selectedFigure), StartPoint.Point, segment, nextSegment);
        }

        public bool CanConvertSelectedArcSegment()
        {
            if (_selectedFigure is ArcSegmentFigure arcSegment && !RulerCreator.IsContinue)
            {
                return true;
            }

            return false;
        }

        public void ConvertSelectedArcSegment()
        {
            if (!CanConvertSelectedArcSegment())
                return;

            var arcSegment = _selectedFigure as ArcSegmentFigure;

            Actions.ConvertArcSegment(this, GetIndex(arcSegment), arcSegment);
        }

        public PathGeometry GetPathGeometry()
        {
            var start = StartPoint.Point;

            var segments = new PathSegmentCollection();

            foreach (var segment in _segments)
                segments.Add(segment.GetSegment());

            var figures = new PathFigure(start, segments, IsClosed);

            var pathRuler = new PathGeometry(new[] {figures});

            return pathRuler;
        }

        public void Dispose()
        {
            RulerCreator.SelectHelper.OnSelectedHelper();

            RulerCreator.DrawingControl.Canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
            RulerCreator.DrawingControl.Canvas.MouseMove -= Canvas_MouseMove;
            RulerCreator.DrawingControl.Canvas.MouseLeftButtonUp -= Canvas_MouseLeftButtonUp;

            RulerCreator.CoordinateSystem.CoordinateChanged -= CoordinateSystem_CoordinateChanged;

            Keyboard.RemovePreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
            Keyboard.RemovePreviewKeyUpHandler(Application.Current.MainWindow, KeyUp);

            ShowMessage(null);

            StartPoint?.Dispose();

            foreach (var segment in _segments)
                segment.Dispose();

            _previewSegment?.Dispose();


            RulerCreator.DrawingControl.Canvas.RemoveElements(_mousePoint, _filledPath,
                _helpMovingLine);

            _closedSegment?.Dispose();
        }

        #endregion

        #region Internal Methods

        internal void CreateStartPoint(Point point)
        {
            StartPoint = CreateStartPointFigure(point, this);

            StartPoint.IsEdgePoint = true;

            _mousePoint.Visibility = Visibility.Collapsed;
            _startPointSetted = true;

            Continue();
        }

        internal void RemoveStartPoint()
        {
            _startPointSetted = false;

            StartPoint?.Dispose();
            StartPoint = null;

            _previewSegment?.Dispose();
            _previewSegment = null;

            _mousePoint.Visibility = Visibility.Visible;

            ShowMessage(HelpMessages.SetStartPointMessage);

            UpdateFilledAndClosed();
        }

        internal void AddArcSegment(Point middleArcPoint, Point endPoint, bool startPointIsSelected)
        {
            if (startPointIsSelected)
            {
                var arcSegment = CreateArcSegment(middleArcPoint, StartPoint.Point);

                arcSegment.PreviewSegment = null;
                arcSegment.PointFigure.IsEdgePoint = false;

                StartPoint.Point = endPoint;
                SelectFigure(StartPoint);

                _segments[0].PreviewSegment = arcSegment;

                _segments.Insert(0, arcSegment);
                arcSegment.Update();
            }
            else
            {
                var lastSegment = _segments.LastOrDefault();

                if (lastSegment != null)
                {
                    lastSegment.PointFigure.IsEdgePoint = false;
                }

                var arcSegment = CreateArcSegment(middleArcPoint, endPoint);

                SelectFigure(arcSegment.PointFigure);

                _segments.Add(arcSegment);
            }

            _previewSegment?.Dispose();
            _previewSegment = null;

            Continue();
        }

        internal void AddLineSegment(Point endPoint, bool startPointIsSelected)
        {
            if (startPointIsSelected)
            {
                var lineSegment = CreateLineSegment(StartPoint.Point, null, false);

                StartPoint.Point = endPoint;
                SelectFigure(StartPoint);

                _segments[0].PreviewSegment = lineSegment;

                _segments.Insert(0, lineSegment);

                lineSegment.Update();
            }
            else
            {
                var lastSegment = _segments.LastOrDefault();

                if (lastSegment != null)
                {
                    lastSegment.PointFigure.IsEdgePoint = false;
                }

                var lineSegment = CreateLineSegment(endPoint, _segments.LastOrDefault());

                SelectFigure(lineSegment.PointFigure);

                _segments.Add(lineSegment);
            }


            _previewSegment?.Dispose();
            _previewSegment = null;

            Continue();
        }

        internal void RemoveLastSegment(bool startPointIsSelected)
        {
            if (startPointIsSelected)
            {
                StartPoint.Point = _segments[0].PointFigure.Point;
                _segments[1].PreviewSegment = null;

                _segments[0].Dispose();
                _segments.RemoveAt(0);

                SelectFigure(StartPoint);
            }
            else
            {
                _segments[^1].Dispose();
                _segments.RemoveAt(_segments.Count - 1);

                var lastSegment = _segments.LastOrDefault();

                if (lastSegment != null)
                {
                    lastSegment.PointFigure.IsEdgePoint = true;
                    SelectFigure(lastSegment.PointFigure);
                }
            }

            _previewSegment?.Dispose();
            _previewSegment = null;

            Continue();
        }

        internal void MovePoint(int index, Point point)
        {
            if (index == 0)
            {
                StartPoint.Point = point;

                var firstSegment = _segments.FirstOrDefault();
                firstSegment?.Update();

                if (firstSegment is ArcSegmentFigure arcSegment)
                {
                    if (!arcSegment.IsValidPrevPoint(point))
                        return;
                }
            }
            else
            {
                var segment = _segments[index - 1];

                if (segment is ArcSegmentFigure arcSegment)
                {
                    if (!arcSegment.IsValidNewPoint(point))
                        return;
                }

                var nextSegment = _segments.Count > index ? _segments[index] : null;

                if (nextSegment is ArcSegmentFigure arcSegment2)
                {
                    if (!arcSegment2.IsValidPrevPoint(point))
                        return;
                }

                segment.PointFigure.Point = point;

                segment?.Update();
                nextSegment?.Update();
            }

            UpdateFilledAndClosed();
        }

        internal void MoveMiddleArcPoint(int index, Point position)
        {
            if (!(_segments[index] is ArcSegmentFigure arcSegment))
                return;

            arcSegment.MiddlePoint = position;

            arcSegment?.Update();
            UpdateFilledAndClosed();
        }

        internal void MoveSegment(int index, Point centerPoint)
        {
            SegmentFigure prevSegment;
            SegmentFigure nextSegment;
            Point prevPoint;
            Point nextPoint;

            if (index == -1)
            {
                prevSegment = _segments[^1];
                nextSegment = _segments[0];

                prevPoint = _closedSegment.Point1;
                nextPoint = _closedSegment.Point2;
            }
            else
            {
                prevSegment = index > 0 ? _segments[index - 1] : null;
                nextSegment = _segments.Count - 1 > index ? _segments[index + 1] : null;

                var segment = _segments[index];
                segment.CalcEdgePoints(centerPoint, out prevPoint, out nextPoint);
            }

            if (prevSegment is ArcSegmentFigure arcSegment)
            {
                if (!arcSegment.IsValidNewPoint(prevPoint))
                    return;
            }

            if (nextSegment is ArcSegmentFigure arcSegment2)
            {
                if (!arcSegment2.IsValidPrevPoint(nextPoint))
                    return;
            }

            if (index == -1)
            {
                var oldCenterPoint = _closedSegment.GetCenterPoint();

                var vector = centerPoint - oldCenterPoint;

                StartPoint.Point += vector;

                _segments[^1].PointFigure.Point += vector;
            }
            else
            {
                var segment = _segments[index];

                segment.SetCenterPoint(centerPoint);
            }

            prevSegment?.Update();
            nextSegment?.Update();

            UpdateFilledAndClosed();
        }

        internal void DeleteSelectedNode(int index)
        {
            if (index == 0)
            {
                StartPoint.Point = _segments[0].PointFigure.Point;
                StartPoint.IsSelected = false;

                _segments[0].Dispose();

                _segments.RemoveAt(0);
                _segments[0].PreviewSegment = null;
            }
            else
            {
                var segment = _segments[index - 1];
                var nextSegment = _segments.Count > index ? _segments[index] : null;
                var prevSegment = index - 2 >= 0 ? _segments[index - 2] : null;

                segment.Dispose();
                _segments.Remove(segment);

                if (nextSegment != null)
                {
                    nextSegment.Dispose();

                    var point = nextSegment.PointFigure.Point;

                    _segments.Remove(nextSegment);

                    var newSegment = CreateLineSegment(point, prevSegment, false);

                    _segments.Insert(index - 1, newSegment);

                    if (_segments[^1] != newSegment)
                    {
                        _segments[index].PreviewSegment = newSegment;
                    }
                    else
                    {
                        newSegment.PointFigure.IsEdgePoint = true;
                    }
                }
                else
                {
                    if (prevSegment != null)
                        prevSegment.PointFigure.IsEdgePoint = true;
                }
            }

            _selectedFigure = null;
            SelectedChanged?.Invoke(this, EventArgs.Empty);

            UpdateFilledAndClosed();
        }

        internal void UnDeleteSelectedNode(int index, Point startPoint, SegmentFigure segment,
            SegmentFigure nextSegment)
        {
            if (index == 0)
            {
                StartPoint.Point = startPoint;

                nextSegment.UnDispose();

                _segments[0].PreviewSegment = nextSegment;
                _segments.Insert(0, nextSegment);
            }
            else
            {
                var prevSegment = index >= 2 ? _segments[index - 2] : null;

                segment.UnDispose();
                segment.PreviewSegment = prevSegment;

                if (nextSegment != null)
                {
                    _segments[index - 1].Dispose();
                    _segments.RemoveAt(index - 1);

                    _segments.Insert(index - 1, segment);

                    nextSegment.UnDispose();
                    nextSegment.PreviewSegment = segment;

                    _segments.Insert(index, nextSegment);

                    if (_segments[^1] != nextSegment)
                    {
                        _segments[index + 1].PreviewSegment = nextSegment;
                        nextSegment.PointFigure.IsEdgePoint = false;
                    }
                    else
                    {
                        nextSegment.PointFigure.IsEdgePoint = true;
                    }

                    segment.PointFigure.IsEdgePoint = false;
                }
                else
                {
                    segment.PointFigure.IsEdgePoint = true;

                    _segments.Add(segment);

                    if (prevSegment != null)
                        prevSegment.PointFigure.IsEdgePoint = false;
                }
            }

            _selectedFigure = null;
            SelectedChanged?.Invoke(this, EventArgs.Empty);

            UpdateFilledAndClosed();
        }

        internal void ConvertArcToLine(int index, ArcSegmentFigure arcSegment)
        {
            var prevSegment = index > 0 ? _segments[index - 1] : null;
            var nextSegment = _segments.Count - 1 > index ? _segments[index + 1] : null;

            var lineSegment = CreateLineSegment(arcSegment.PointFigure.Point, prevSegment, nextSegment == null);
            if (nextSegment != null)
            {
                nextSegment.PreviewSegment = lineSegment;
            }

            _segments[index].Dispose();

            _segments[index] = lineSegment;

            prevSegment?.Update();
            nextSegment?.Update();

            _selectedFigure = null;
            SelectedChanged?.Invoke(this, EventArgs.Empty);

            UpdateFilledAndClosed();
        }

        internal void ConvertLineToArc(int index, ArcSegmentFigure arcSegment)
        {
            var segment = _segments[index];
            var prevSegment = index > 0 ? _segments[index - 1] : null;
            var nextSegment = _segments.Count - 1 > index ? _segments[index + 1] : null;

            segment.Dispose();

            arcSegment.PreviewSegment = prevSegment;
            arcSegment.PointFigure.IsEdgePoint = nextSegment == null;

            arcSegment.UnDispose();

            if (nextSegment != null)
            {
                nextSegment.PreviewSegment = arcSegment;
            }

            _segments[index] = arcSegment;

            SelectFigure(arcSegment.HelpPointFigure);

            prevSegment?.Update();
            nextSegment?.Update();

            _selectedFigure = null;
            SelectedChanged?.Invoke(this, EventArgs.Empty);

            UpdateFilledAndClosed();
        }

        internal void SplitLineSegment(int index, Point newPoint)
        {
            var segment = _segments[index];
            var nextSegment = _segments.Count - 1 > index ? _segments[index + 1] : null;

            var newSegment = CreateLineSegment(segment.PointFigure.Point, segment, segment.PointFigure.IsEdgePoint);

            _segments.Insert(index + 1, newSegment);

            if (nextSegment != null)
            {
                nextSegment.PreviewSegment = newSegment;
            }
            else
            {
                segment.PointFigure.IsEdgePoint = false;
                newSegment.PointFigure.IsEdgePoint = true;
            }

            segment.PointFigure.Point = newPoint;

            SelectFigure(segment.PointFigure);

            segment?.Update();
            nextSegment?.Update();
            newSegment?.Update();

            UpdateFilledAndClosed();
        }

        internal void UnSplitLineSegment(int index)
        {
            var segment = _segments[index];
            var nextSegment = _segments.Count - 1 > index ? _segments[index + 1] : null;
            var next2Segment = _segments.Count - 2 > index ? _segments[index + 2] : null;

            segment.PointFigure.Point = nextSegment.PointFigure.Point;

            _segments.Remove(nextSegment);

            nextSegment?.Dispose();

            if (next2Segment != null)
            {
                next2Segment.PreviewSegment = segment;
            }
            else
            {
                segment.PointFigure.IsEdgePoint = true;
            }

            segment?.Update();
            next2Segment?.Update();

            UpdateFilledAndClosed();
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            RulerCreator.FigureCollection.UnselectAllFigures();

            RulerCreator.SelectHelper.OffSelectedHelper();

            RulerCreator.DrawingControl.Canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            RulerCreator.DrawingControl.Canvas.MouseMove += Canvas_MouseMove;
            RulerCreator.DrawingControl.Canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            RulerCreator.CoordinateSystem.CoordinateChanged += CoordinateSystem_CoordinateChanged;

            Keyboard.AddPreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
            Keyboard.AddPreviewKeyUpHandler(Application.Current.MainWindow, KeyUp);

            _mousePoint = ShapeFactory.CreateMouseCreatePoint();
            _mousePoint.AddToCanvas(RulerCreator.DrawingControl.Canvas);

            _helpMovingLine = ShapeFactory.CreateHelpLine();
            _helpMovingLine.AddToCanvas(RulerCreator.DrawingControl.Canvas);
            
            _filledPath = ShapeFactory.CreateFilledArea();
            _filledPath.AddToCanvas(RulerCreator.DrawingControl.Canvas);

            _closedSegment = new ClosedSegmentFigure(this);

            if (RulerCreator.IsClosed)
                _closedSegment.IsVisibility = true;
        }

        private void EditingAction()
        {
            var Ruler = RulerCreator.Figure.Geometry;
            var pathRuler = Ruler as PathGeometry;
            IsClosed = pathRuler.IsClosed();

            CreateStartPoint(pathRuler.GetStartPoint());

            var segments = pathRuler.GetSegments();

            foreach (var segment in segments)
            {
                switch (segment)
                {
                    case ArcSegment arcSegment:
                        AddArcSegment(arcSegment);
                        break;
                    case LineSegment lineSegment:
                        AddLineSegment(lineSegment.Point, false);
                        break;
                    case PolyLineSegment polyLineSegment:
                        foreach (var point in polyLineSegment.Points)
                            AddLineSegment(point, false);
                        break;
                }
            }
        }

        private void AddArcSegment(ArcSegment arcSegment)
        {
            var lastSegment = _segments.LastOrDefault();

            if (lastSegment != null)
            {
                lastSegment.PointFigure.IsEdgePoint = false;
            }

            var arcSegmentFigure = CreateArcSegment(arcSegment);

            SelectFigure(arcSegmentFigure.PointFigure);

            _segments.Add(arcSegmentFigure);

            _previewSegment?.Dispose();
            _previewSegment = null;

            Continue();
        }

        private void CreatePreviewSegment()
        {
            if (RulerCreator.IsContinue && _startPointSetted)
            {
                if (!(_previewSegment is PreviewLineSegmentFigure))
                {
                    _previewSegment?.Dispose();
                    _previewSegment =
                        CreatePreviewLineSegmentFigure(
                            RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointShiftMode));
                }
            }
            else
            {
                _previewSegment?.Dispose();
                _previewSegment = null;
            }
        }

        private void CreateArcPreviewSegment()
        {
            if (GeometryMath.IsPointsEqual(RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointShiftMode),
                _previewSegment.PrevPoint))
                return;
            _middleArcPoint = RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointShiftMode);
            _middleArcPointSetted = true;
            ShowMessage(HelpMessages.SetEndPointArcMessage);

            if (_previewSegment is PreviewLineSegmentFigure)
            {
                _previewSegment.Dispose();
                _previewSegment = null;
            }

            _previewSegment =
                CreatePreviewArcSegmentFigure(_middleArcPoint,
                    RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointWithoutShiftMode));

            UpdatePreviewSegments();
        }

        private void UpdateMessages()
        {
            if (!RulerCreator.IsCreating) return;

            if (RulerCreator.IsClosed)
            {
                if (_segments.Count < 1)
                    return;
            }

            var sumLength = _segments.Sum(x => x.Length);
            var allLength = sumLength;

            var message = "";

            if (RulerCreator.IsContinue && _previewSegment != null)
                allLength += _previewSegment.Length;

            if (RulerCreator.IsClosed)
            {
                allLength += _closedSegment.Length;

                var endSegmLength = (StartPoint.Point - _segments.Last().PointFigure.Point).Length;
                sumLength += endSegmLength;

                message += $"Периметр: {sumLength.ToStr()} ({allLength.ToStr()})";
            }
            else
            {
                message += $"Длина: {sumLength.ToStr()} ({allLength.ToStr()})";
            }

            if (RulerCreator.IsClosed && _segments.Count > 0)
            {
                message +=
                    $"\r\nПлощадь: {GetPathGeometry().GetArea().ToAreaStr()} ({GetFilledPathGeometry().GetArea().ToAreaStr()})";
            }

            ShowMessage(message);
        }

        private void CoordinateSystem_CoordinateChanged(object sender, CoordinateChangedArgs e)
        {
            StartPoint?.Update();
            foreach (var segmentFigure in _segments)
                segmentFigure.Update();

            UpdatePreviewSegments();
        }

        private void UpdatePreviewSegments()
        {
            if (_previewSegment is PreviewLineSegmentFigure)
                _previewSegment.UpdatePoint(RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointShiftMode));

            else if (_previewSegment is PreviewArcSegmentFigure && !GeometryMath.IsThreePointsOnOneLine(_middleArcPoint,
                RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointWithoutShiftMode),
                _previewSegment.PrevPoint))
                _previewSegment.UpdatePoint(
                    RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointWithoutShiftMode));

            UpdateFilledAndClosed();
        }

        private void CalcMagnetPoints(Point canvasPosition)
        {
            _magnetPointWithoutShiftMode = CalcMagnetPoint(canvasPosition);

            var prevPoint = new Point();

            if (_segments.Any() && !StartPoint.IsSelected)
            {
                prevPoint = _segments.Last().PointFigure.Point;
            }
            else if (StartPoint != null)
            {
                prevPoint = StartPoint.Point;
            }

            _magnetPointShiftMode =
                CalcMagnetPoint(canvasPosition, true, RulerCreator.CoordinateSystem.GetCanvasPoint(prevPoint));
        }

        private SegmentFigure SearchSegment(PointFigure pointFigure, out SegmentFigure nextSegment,
            out SegmentFigure prevSegment)
        {
            if (StartPoint == pointFigure)
            {
                nextSegment = _segments.FirstOrDefault();
                prevSegment = null;

                return null;
            }

            for (var i = 0; i < _segments.Count; i++)
            {
                var segment = _segments[i];
                if (segment.PointFigure == pointFigure)
                {
                    nextSegment = i < _segments.Count - 1 ? _segments[i + 1] : null;

                    prevSegment = i > 0 ? _segments[i - 1] : null;

                    return segment;
                }
            }

            nextSegment = null;
            prevSegment = null;
            return null;
        }

        private int GetIndex(PointFigure movedPoint)
        {
            if (movedPoint == StartPoint)
                return 0;

            for (var i = 0; i < _segments.Count; i++)
            {
                if (_segments[i].PointFigure == movedPoint)
                    return i + 1;
            }

            return 0;
        }

        private int GetIndex(SegmentFigure segment)
        {
            for (var i = 0; i < _segments.Count; i++)
            {
                if (segment == _segments[i])
                    return i;
            }

            return 0;
        }

        private int GetIndex(HelpPointFigure movedPoint)
        {
            for (var i = 0; i < _segments.Count; i++)
            {
                if (movedPoint == _segments[i].HelpPointFigure)
                    return i;
            }

            return 0;
        }

        private void UpdateHelpLine(Point centerPoint, Vector ortVector)
        {
            var canvasCenterPoint = RulerCreator.CoordinateSystem.GetCanvasPoint(centerPoint);

            var p1 = canvasCenterPoint - ortVector * 100000;
            var p2 = canvasCenterPoint + ortVector * 100000;

            if (ortVector == new Vector(0, 1) || ortVector == new Vector(0, -1))
            {
                p1 = new Point(canvasCenterPoint.X, 0);
                p2 = new Point(canvasCenterPoint.X, RulerCreator.DrawingControl.Canvas.ActualHeight);
            }
            else if (ortVector == new Vector(1, 0) || ortVector == new Vector(-1, 0))
            {
                p1 = new Point(0, canvasCenterPoint.Y);
                p2 = new Point(RulerCreator.DrawingControl.Canvas.ActualWidth, canvasCenterPoint.Y);
            }

            _helpMovingLine.SetPoints(p1.X, p1.Y, p2.X, p2.Y);
        }

        private void UpdateHelpLine(SegmentFigure segment)
        {
            var index = GetIndex(segment);
            var prevSegment = index > 0 ? _segments[index - 1] : null;

            var point1 = prevSegment?.PointFigure.Point ?? StartPoint.Point;
            var point2 = segment.PointFigure.Point;
            var vector = point2 - point1;

            var ortVector = new Vector(vector.Y, vector.X);
            ortVector.Normalize();

            var centerPoint = new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);

            UpdateHelpLine(centerPoint, ortVector);
        }

        private void UpdateHelpLine(ClosedSegmentFigure closedSegment)
        {
            var point1 = closedSegment.Point1;
            var point2 = closedSegment.Point2;
            var vector = point2 - point1;

            var ortVector = new Vector(vector.Y, vector.X);
            ortVector.Normalize();

            var centerPoint = new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);

            UpdateHelpLine(centerPoint, ortVector);
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                if (_selectedFigure is SegmentFigure segment)
                {
                    _helpMovingLine.Visibility = Visibility.Visible;

                    UpdateHelpLine(segment);
                }

                if (_selectedFigure is ClosedSegmentFigure closedSegment)
                {
                    _helpMovingLine.Visibility = Visibility.Visible;

                    UpdateHelpLine(closedSegment);
                }
            }
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                _helpMovingLine.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateHelpPoints()
        {
            for (var index = 0; index < _segments.Count; index++)
            {
                var segment = _segments[index];

                segment.HelpPointFigure?.Update();
            }
        }

        private Point CalcMovedCenterSegmentPoint(int index, SegmentFigure segment, Point globalMousePos, Point point)
        {
            var prevSegment = index > 0 ? _segments[index - 1] : null;

            var point1 = prevSegment?.PointFigure.Point ?? StartPoint.Point;
            var point2 = segment.PointFigure.Point;

            var centerPoint = new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
            var vector = point2 - point1;
            var ortVector = new Vector(vector.Y, vector.X);
            ortVector.Normalize();


            var hitPoint = centerPoint + _movedOffsetVector;

            var intersectPoint = GetIntersectionPointOfTwoLines(
                hitPoint + new Vector(-vector.Y, vector.X) * 10,
                hitPoint - new Vector(-vector.Y, vector.X) * 10,
                globalMousePos + vector * 10,
                globalMousePos - vector * 10,
                out var state);

            if (state == 1)
            {
                point = intersectPoint - _movedOffsetVector;
            }

            UpdateHelpLine(centerPoint, ortVector);
            return point;
        }

        private Point CalcMovedCenterSegmentPoint(Point globalMousePos, Point point)
        {
            var point1 = _closedSegment.Point1;
            var point2 = _closedSegment.Point2;

            var centerPoint = new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
            var vector = point2 - point1;
            var ortVector = new Vector(vector.Y, vector.X);
            ortVector.Normalize();


            var hitPoint = centerPoint + _movedOffsetVector;

            var intersectPoint = GetIntersectionPointOfTwoLines(
                hitPoint + new Vector(-vector.Y, vector.X) * 10,
                hitPoint - new Vector(-vector.Y, vector.X) * 10,
                globalMousePos + vector * 10,
                globalMousePos - vector * 10,
                out var state);

            if (state == 1)
            {
                point = intersectPoint - _movedOffsetVector;
            }

            UpdateHelpLine(centerPoint, ortVector);
            return point;
        }

        #region Select & Highlight

        private void HighlightPointsAndSegments(Point pos)
        {
            var globalPos = RulerCreator.CoordinateSystem.GetGlobalPoint(pos);

            for (var i = _segments.Count - 1; i >= 0; i--)
            {
                _segments[i].Highlight(false);
                _segments[i].PointFigure.Highlight(false);
                _segments[i].HelpPointFigure?.Highlight(false);
            }

            StartPoint?.Highlight(false);
            _closedSegment?.Highlight(false);

            var radius = RulerCreator.CoordinateSystem.GetGlobalLength(2);
            var figure = FindHitFigure(new EllipseGeometry(new Point(globalPos.X, globalPos.Y), radius, radius));

            figure?.Highlight(true);
        }

        private Figure SelectPoints(Point point)
        {
            var globalPos = RulerCreator.CoordinateSystem.GetGlobalPoint(point);

            var radius = RulerCreator.CoordinateSystem.GetGlobalLength(2);

            var figure = FindHitFigure(new EllipseGeometry(new Point(globalPos.X, globalPos.Y), radius, radius));

            return SelectFigure(figure);
        }

        private Figure SelectFigure(Figure figure)
        {
            for (var i = _segments.Count - 1; i >= 0; i--)
            {
                _segments[i].PointFigure.IsSelected = false;

                _segments[i].IsSelected = false;

                if (_segments[i].HelpPointFigure != null)
                    _segments[i].HelpPointFigure.IsSelected = false;
            }

            if (StartPoint != null)
                StartPoint.IsSelected = false;

            if (_closedSegment != null)
                _closedSegment.IsSelected = false;

            _selectedFigure = null;

            if (figure != null)
            {
                figure.IsSelected = true;
                _selectedFigure = figure;
            }

            SelectedChanged?.Invoke(this, EventArgs.Empty);

            return figure;
        }

        #endregion

        #region Mouse Actions

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_startPointSetted)
                Actions.AddStartPoint(this, _magnetPointWithoutShiftMode);

            else if (RulerCreator.IsContinue)
            {
                if (RulerCreator.CreationMode == CreationMode.Line)
                    Actions.AddLineSegment(this, _magnetPointShiftMode, StartPoint.IsSelected);

                else if (RulerCreator.CreationMode == CreationMode.Arc)
                {
                    if (!_middleArcPointSetted)
                        CreateArcPreviewSegment();
                    else
                    {
                        var temp = RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointWithoutShiftMode);
                        if (!GeometryMath.IsThreePointsOnOneLine(_middleArcPoint, temp, _previewSegment.PrevPoint))
                            //todo: refactoring , добавить проверку на конечную точку дуги между начальной и серединной
                            Actions.AddArcSegment(this, _middleArcPoint, _magnetPointWithoutShiftMode,
                                StartPoint.IsSelected);
                    }
                }
            }
            else
            {
                var mousePos = e.GetPosition(RulerCreator.DrawingControl.Canvas);

                _movedFigure = SelectPoints(mousePos);

                switch (_movedFigure)
                {
                    case PointFigure movedPoint:
                        _movedPointOldValue = movedPoint.Point;
                        break;
                    case HelpPointFigure _:
                        _movedPointOldValue = RulerCreator.CoordinateSystem.GetGlobalPoint(mousePos);
                        break;
                    case SegmentFigure segment:
                        _movedSegmentCenterOldValue = segment.GetCenterPoint();
                        _movedOffsetVector =
                            RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointWithoutShiftMode) -
                            _movedSegmentCenterOldValue;
                        break;
                    case ClosedSegmentFigure closedSegment when IsClosed:

                        _movedSegmentCenterOldValue = closedSegment.GetCenterPoint();
                        _movedOffsetVector =
                            RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointWithoutShiftMode) -
                            _movedSegmentCenterOldValue;

                        break;
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(RulerCreator.DrawingControl.Canvas);

            CalcMagnetPoints(pos);

            if (!_startPointSetted)
                _mousePoint.SetPosition(_magnetPointWithoutShiftMode);

            if (RulerCreator.IsContinue)
                UpdatePreviewSegments();
            else
            {
                var globalMousePos = RulerCreator.CoordinateSystem.GetGlobalPoint(_magnetPointWithoutShiftMode);

                if (_movedFigure is PointFigure movedPoint)
                    MovePoint(GetIndex(movedPoint), globalMousePos);
                else if (_movedFigure is SegmentFigure segment)
                {
                    var point = globalMousePos - _movedOffsetVector;

                    var index = GetIndex(segment);

                    if (Keyboard.IsKeyDown(Key.LeftShift))
                        point = CalcMovedCenterSegmentPoint(index, segment, globalMousePos, point);

                    MoveSegment(index, point);
                }
                else if (_movedFigure is ClosedSegmentFigure && IsClosed)
                {
                    var point = globalMousePos - _movedOffsetVector;

                    const int index = -1;

                    if (Keyboard.IsKeyDown(Key.LeftShift))
                        point = CalcMovedCenterSegmentPoint(globalMousePos, point);

                    MoveSegment(index, point);
                }
                else if (_movedFigure is HelpPointFigure helpPoint)
                {
                    var index = GetIndex(helpPoint);
                    var currentSegment = _segments[index];

                    var globalPos = RulerCreator.CoordinateSystem.GetGlobalPoint(pos);

                    if (currentSegment is LineSegmentFigure lineSegment &&
                        globalPos != _movedPointOldValue)
                    {
                        if (RulerCreator.CreationMode == CreationMode.Line)
                        {
                            SplitLineSegment(index, globalPos);

                            _movedFigure = lineSegment.PointFigure;

                            _lineSplitted = true;
                        }
                        else
                        {
                            var arcSegment = CreateArcSegment(globalPos, lineSegment.PointFigure.Point);

                            arcSegment.Dispose();

                            ConvertLineToArc(index, arcSegment);

                            _movedFigure = arcSegment.HelpPointFigure;

                            _lineConverted = true;
                        }
                    }
                    else if (currentSegment is ArcSegmentFigure)
                        MoveMiddleArcPoint(index, globalMousePos);
                }
                else
                {
                    HighlightPointsAndSegments(pos);
                }

                UpdateMessages();
            }
        }


        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_movedFigure is PointFigure movedPoint)
            {
                if (movedPoint.Point != _movedPointOldValue)
                {
                    if (_lineSplitted)
                    {
                        var point = movedPoint.Point;

                        UnSplitLineSegment(GetIndex(movedPoint) - 1);

                        Actions.SplitLine(this, GetIndex(movedPoint) - 1, point);

                        _lineSplitted = false;
                    }
                    else
                    {
                        Actions.MovePoint(this, GetIndex(movedPoint), movedPoint.Point, _movedPointOldValue);
                    }
                }

                _movedFigure = null;
            }
            else if (_movedFigure is SegmentFigure segment)
            {
                if (segment.GetCenterPoint() != _movedSegmentCenterOldValue)
                {
                    Actions.MoveSegment(this, GetIndex(segment), segment.GetCenterPoint(), _movedSegmentCenterOldValue);
                    segment.Update();
                }

                _helpMovingLine.Visibility = Visibility.Collapsed;
                _movedFigure = null;
            }
            else if (_movedFigure is ClosedSegmentFigure && IsClosed)
            {
                if (_closedSegment.GetCenterPoint() != _movedSegmentCenterOldValue)
                {
                    Actions.MoveSegment(this, -1, _closedSegment.GetCenterPoint(), _movedSegmentCenterOldValue);
                }

                _helpMovingLine.Visibility = Visibility.Collapsed;
                _movedFigure = null;
            }
            else if (_movedFigure is HelpPointFigure helpPoint)
            {
                var index = GetIndex(helpPoint);
                var currentSegment = _segments[index];

                if (currentSegment is ArcSegmentFigure arcSegment)
                {
                    if (arcSegment.HelpPointFigure.Point != _movedPointOldValue)
                    {
                        if (_lineConverted)
                        {
                            Actions.ConvertLineToArcSegment(this, GetIndex(arcSegment), arcSegment);

                            _lineConverted = false;
                        }
                        else
                        {
                            Actions.MoveArcMiddlePoint(this, GetIndex(arcSegment), arcSegment.HelpPointFigure.Point,
                                _movedPointOldValue);
                        }
                    }
                }

                _movedFigure = null;
            }
        }

        #endregion

        #region Fill & Closed

        private void UpdateFilledAndClosed()
        {
            UpdateFilledArea();
            UpdateClosedSegment();
            UpdateMessages();
        }

        private void UpdateFilledArea()
        {
            if (!_segments.Any() || !IsClosed)
            {
                if (_filledPath != null)
                    _filledPath.Data = null;

                return;
            }

            var ruler = GetFilledPathGeometry();

            _filledPath.Data = RulerCreator.CoordinateSystem.GetCanvasGeometry(ruler);
        }

        /// <summary>
        /// Получение полной геометрии, включая preview сегменты и замыкающий сегмент
        /// </summary>
        /// <returns></returns>
        private PathGeometry GetFilledPathGeometry()
        {
            var ruler = GetPathGeometry();

            ruler.Figures.First().IsClosed = true;

            if (_previewSegment != null)
            {
                if (StartPoint.IsSelected)
                {
                    ruler.Figures.First().StartPoint = _previewSegment.EndPoint;

                    var segment = _previewSegment.GetSegment(true);

                    ruler.Figures.First().Segments.Insert(0, segment);
                }
                else
                {
                    ruler.Figures.First().Segments.Add(_previewSegment.GetSegment());
                }
            }

            return ruler;
        }

        private void UpdateClosedSegment()
        {
            if (!IsClosed || !_startPointSetted || !_segments.Any())
            {
                _closedSegment.IsVisibility = false;
                _closedSegment.LengthTextBlock.Visibility = Visibility.Hidden;

                return;
            }

            _closedSegment.LengthTextBlock.Visibility = Visibility.Visible;
            _closedSegment.IsVisibility = true;
            _closedSegment.Update();

            _closedSegment.Point1 = StartPoint.Point;

            if (_previewSegment != null)
            {
                _closedSegment.Point1 = StartPoint.IsSelected
                    ? _segments.Last().PointFigure.Point
                    : StartPoint.Point;

                _closedSegment.Point2 = _previewSegment.EndPoint;
            }
            else
            {
                if (_segments.Any())
                {
                    _closedSegment.Point2 = _segments.Last().PointFigure.Point;
                }
            }
        }

        #endregion

        #region Hit Test

        private Figure FindHitFigure(Geometry intersectRuler)
        {
            for (var i = _segments.Count - 1; i >= 0; i--)
            {
                var Ruler = _segments[i].PointFigure.GetGeometry();

                if (Ruler.IsEmpty()) continue;

                var detail = Ruler.FillContainsWithDetail(intersectRuler);

                if (!(detail == IntersectionDetail.Empty ||
                      detail == IntersectionDetail.NotCalculated))
                    return _segments[i].PointFigure;

                Ruler = _segments[i].HelpPointFigure?.GetGeometry();

                if (Ruler == null || Ruler.IsEmpty()) continue;

                detail = Ruler.FillContainsWithDetail(intersectRuler);

                if (!(detail == IntersectionDetail.Empty ||
                      detail == IntersectionDetail.NotCalculated))
                    return _segments[i].HelpPointFigure;
            }

            if (StartPoint != null)
            {
                var Ruler = StartPoint.GetGeometry();

                if (!Ruler.IsEmpty())
                {
                    var detail = Ruler.FillContainsWithDetail(intersectRuler);

                    if (!(detail == IntersectionDetail.Empty ||
                          detail == IntersectionDetail.NotCalculated))
                        return StartPoint;
                }
            }

            for (var i = _segments.Count - 1; i >= 0; i--)
            {
                var Ruler = _segments[i].GetGeometry();

                if (Ruler.IsEmpty()) continue;

                var detail = Ruler.StrokeContainsWithDetail(new Pen(Brushes.Red, 1), intersectRuler);

                if (!(detail == IntersectionDetail.Empty ||
                      detail == IntersectionDetail.NotCalculated))
                    return _segments[i];
            }

            if (_closedSegment != null)
            {
                var Ruler = _closedSegment.GetGeometry();

                if (!Ruler.IsEmpty())
                {
                    var detail = Ruler.StrokeContainsWithDetail(new Pen(Brushes.Red, 1), intersectRuler);

                    if (!(detail == IntersectionDetail.Empty ||
                          detail == IntersectionDetail.NotCalculated))
                        return _closedSegment;
                }
            }

            return null;
        }

        #endregion

        #region Helpers

        private void ShowMessage(string message, bool isWarning = false) =>
            RulerCreator.DrawingControl.ShowMessage(message, isWarning);

        private Point CalcMagnetPoint(Point point, bool isShiftMode = false, Point prevPoint = new Point()) =>
            point.GetMagnetPoint(RulerCreator.Settings, RulerCreator.GridChart,
                RulerCreator.FigureCollection, RulerCreator.CoordinateSystem,
                isShiftMode && Keyboard.IsKeyDown(Key.LeftShift), prevPoint);

        /// <summary>
        /// Возвращает точку пересечения двух прямых
        /// </summary>
        /// <param name="p1_1">Первая точка прямой 1</param>
        /// <param name="p1_2">Вторая точка прямой 1</param>
        /// <param name="p2_1">Первая точка прямой 2</param>
        /// <param name="p2_2">Вторая точка прямой 2</param>
        /// <param name="state">-1, если параллельны, 0 если совпадают, 1 если пересекаются, -2 если ошибка</param>
        /// <returns></returns>
        public static Point GetIntersectionPointOfTwoLines(Point p1_1, Point p1_2, Point p2_1, Point p2_2,
            out int state)
        {
            state = -2;
            var result = new Point();

            //Если знаменатель (n) равен нулю, то прямые параллельны.
            //Если и числитель (m или w) и знаменатель (n) равны нулю, то прямые совпадают.
            //Если нужно найти пересечение отрезков, то нужно лишь проверить, лежат ли ua и ub на промежутке [0,1].
            //Если какая-нибудь из этих двух переменных 0 <= ui <= 1, то соответствующий отрезок содержит точку пересечения.
            //Если обе переменные приняли значения из [0,1], то точка пересечения прямых лежит внутри обоих отрезков.
            var m = ((p2_2.X - p2_1.X) * (p1_1.Y - p2_1.Y) - (p2_2.Y - p2_1.Y) * (p1_1.X - p2_1.X));
            var w = ((p1_2.X - p1_1.X) * (p1_1.Y - p2_1.Y) -
                     (p1_2.Y - p1_1.Y) * (p1_1.X - p2_1.X)); //Можно обойтись и без этого
            var n = ((p2_2.Y - p2_1.Y) * (p1_2.X - p1_1.X) - (p2_2.X - p2_1.X) * (p1_2.Y - p1_1.Y));

            var Ua = m / n;
            var Ub = w / n;

            if ((n == 0) && (m != 0))
            {
                state = -1; //Прямые параллельны (не имеют пересечения)
            }
            else if ((m == 0) && (n == 0))
            {
                state = 0; //Прямые совпадают
            }
            else
            {
                //Прямые имеют точку пересечения
                result.X = p1_1.X + Ua * (p1_2.X - p1_1.X);
                result.Y = p1_1.Y + Ua * (p1_2.Y - p1_1.Y);

                // Проверка попадания в интервал
                var a = result.X >= p1_1.X;
                var b = result.X <= p1_1.X;
                var c = result.X >= p2_1.X;
                var d = result.X <= p2_1.X;
                var e = result.Y >= p1_1.Y;
                var f = result.Y <= p1_1.Y;
                var g = result.Y >= p2_1.Y;
                var h = result.Y <= p2_1.Y;

                if (((a || b) && (c || d)) && ((e || f) && (g || h)))
                {
                    state = 1; //Прямые имеют точку пересечения
                }
            }

            return result;
        }

        #endregion

        #region Factory

        private static PointFigure CreateStartPointFigure(Point startPoint, RulerWorkspace workspace)
        {
            return new PointFigure(startPoint, workspace);
        }

        private PreviewLineSegmentFigure CreatePreviewLineSegmentFigure(Point startPoint)
        {
            var prevPoint = StartPoint.Point;

            if (_segments.Any())
            {
                var lastSegmentPoint = _segments.Last().PointFigure;
                if (lastSegmentPoint.IsSelected)
                    prevPoint = lastSegmentPoint.Point;
            }

            return new PreviewLineSegmentFigure(startPoint, prevPoint, this);
        }

        private PreviewArcSegmentFigure CreatePreviewArcSegmentFigure(Point middleArcPoint, Point startPoint)
        {
            var prevPoint = StartPoint.Point;

            if (_segments.Any())
            {
                var lastSegmentPoint = _segments.Last().PointFigure;
                if (lastSegmentPoint.IsSelected)
                    prevPoint = lastSegmentPoint.Point;
            }

            return new PreviewArcSegmentFigure(middleArcPoint, startPoint, prevPoint, this);
        }

        private LineSegmentFigure CreateLineSegment(Point endPoint, SegmentFigure previewSegment = null,
            bool isEdgePoint = true)
        {
            var lineSegmentFigure = new LineSegmentFigure(endPoint, previewSegment, this);

            lineSegmentFigure.PointFigure.IsEdgePoint = isEdgePoint;

            return lineSegmentFigure;
        }

        private ArcSegmentFigure CreateArcSegment(Point middlePoint, Point endPoint)
        {
            var arcSegmentFigure = new ArcSegmentFigure(middlePoint, endPoint, _segments.LastOrDefault(), this);

            arcSegmentFigure.PointFigure.IsEdgePoint = true;

            return arcSegmentFigure;
        }

        private ArcSegmentFigure CreateArcSegment(ArcSegment arcSegment)
        {
            var arcSegmentFigure = new ArcSegmentFigure(arcSegment, _segments.LastOrDefault(), this);

            arcSegmentFigure.PointFigure.IsEdgePoint = true;

            return arcSegmentFigure;
        }

        #endregion

        #endregion
    }
}