using System.Windows;

namespace Ariadna.Engine.GeometryCreator
{
    internal static class Actions
    {
        public static void AddStartPoint(GeometryWorkspace workspace, Point startPoint)
        {
            var addStartPointAction =
                new AddStartPointAction(workspace.GeometryCreator.CoordinateSystem.GetGlobalPoint(startPoint),
                    workspace);

            workspace.GeometryCreator.ActionManager.RecordAction(addStartPointAction);
        }

        public static void AddLineSegment(GeometryWorkspace workspace, Point endPoint, bool startPointIsSelected)
        {
            var addLineSegmentAction = new AddLineSegmentAction(
                workspace.GeometryCreator.CoordinateSystem.GetGlobalPoint(endPoint), workspace, startPointIsSelected);

            workspace.GeometryCreator.ActionManager.RecordAction(addLineSegmentAction);
        }

        public static void AddArcSegment(GeometryWorkspace workspace, Point middleArcPoint, Point endPoint,
            bool startPointIsSelected)
        {
            var addArcSegmentAction = new AddArcSegmentAction(middleArcPoint,
                workspace.GeometryCreator.CoordinateSystem.GetGlobalPoint(endPoint), workspace, startPointIsSelected);

            workspace.GeometryCreator.ActionManager.RecordAction(addArcSegmentAction);
        }

        public static void MovePoint(GeometryWorkspace workspace, int index, Point newPoint, Point oldPoint)
        {
            var movePointAction = new MovePointAction(index, oldPoint, newPoint, workspace);
            workspace.GeometryCreator.ActionManager.RecordAction(movePointAction);
        }

        public static void MoveSegment(GeometryWorkspace workspace, int index, Point newCenterPoint,
            Point oldCenterPoint)
        {
            var moveSegmentAction = new MoveSegmentAction(workspace, index, newCenterPoint, oldCenterPoint);
            workspace.GeometryCreator.ActionManager.RecordAction(moveSegmentAction);
        }

        public static void DeleteNode(GeometryWorkspace workspace, int index, Point startPoint, SegmentFigure segment,
            SegmentFigure nextSegment)
        {
            var deleteNodeAction =
                new DeleteNodeAction(workspace, index, startPoint, segment, nextSegment);
            workspace.GeometryCreator.ActionManager.RecordAction(deleteNodeAction);
        }

        public static void ConvertArcSegment(GeometryWorkspace workspace, int index, ArcSegmentFigure arcSegment)
        {
            var convertArcAction = new ConvertArcAction(workspace, index, arcSegment);
            workspace.GeometryCreator.ActionManager.RecordAction(convertArcAction);
        }

        public static void MoveArcMiddlePoint(GeometryWorkspace workspace, int index, Point newMiddlePoint,
            Point oldMiddlePoint)
        {
            var moveArcMiddlePointAction =
                new MoveArcMiddlePointAction(workspace, index, newMiddlePoint, oldMiddlePoint);
            workspace.GeometryCreator.ActionManager.RecordAction(moveArcMiddlePointAction);
        }

        public static void ConvertLineToArcSegment(GeometryWorkspace workspace, int index, ArcSegmentFigure arcSegment)
        {
            var convertLineToArcAction = new ConvertLineToArcAction(workspace, index, arcSegment);

            workspace.GeometryCreator.ActionManager.RecordAction(convertLineToArcAction);
        }

        public static void SplitLine(GeometryWorkspace workspace, int index, Point newPoint)
        {
            var splitLineAction = new SplitLineAction(workspace, index, newPoint);
            workspace.GeometryCreator.ActionManager.RecordAction(splitLineAction);
        }
    }
}