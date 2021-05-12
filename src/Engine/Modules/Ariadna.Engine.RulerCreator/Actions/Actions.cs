using System.Runtime.CompilerServices;
using System.Windows;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal static class Actions
    {
        public static void AddStartPoint(RulerWorkspace workspace, Point startPoint)
        {
            var addStartPointAction =
                new AddStartPointAction(workspace.RulerCreator.CoordinateSystem.GetGlobalPoint(startPoint),
                    workspace);

            workspace.RulerCreator.ActionManager.RecordAction(addStartPointAction);
        }

        public static void AddLineSegment(RulerWorkspace workspace, Point endPoint, bool startPointIsSelected)
        {
            var addLineSegmentAction = new AddLineSegmentAction(
                workspace.RulerCreator.CoordinateSystem.GetGlobalPoint(endPoint), workspace, startPointIsSelected);

            workspace.RulerCreator.ActionManager.RecordAction(addLineSegmentAction);
        }

        public static void AddArcSegment(RulerWorkspace workspace, Point middleArcPoint, Point endPoint,
            bool startPointIsSelected)
        {
            var addArcSegmentAction = new AddArcSegmentAction(middleArcPoint,
                workspace.RulerCreator.CoordinateSystem.GetGlobalPoint(endPoint), workspace, startPointIsSelected);

            workspace.RulerCreator.ActionManager.RecordAction(addArcSegmentAction);
        }

        public static void MovePoint(RulerWorkspace workspace, int index, Point newPoint, Point oldPoint)
        {
            var movePointAction = new MovePointAction(index, oldPoint, newPoint, workspace);
            workspace.RulerCreator.ActionManager.RecordAction(movePointAction);
        }

        public static void MoveSegment(RulerWorkspace workspace, int index, Point newCenterPoint,
            Point oldCenterPoint)
        {
            var moveSegmentAction = new MoveSegmentAction(workspace, index, newCenterPoint, oldCenterPoint);
            workspace.RulerCreator.ActionManager.RecordAction(moveSegmentAction);
        }

        public static void DeleteNode(RulerWorkspace workspace, int index, Point startPoint, SegmentFigure segment,
            SegmentFigure nextSegment)
        {
            var deleteNodeAction =
                new DeleteNodeAction(workspace, index, startPoint, segment, nextSegment);
            workspace.RulerCreator.ActionManager.RecordAction(deleteNodeAction);
        }

        public static void ConvertArcSegment(RulerWorkspace workspace, int index, ArcSegmentFigure arcSegment)
        {
            var convertArcAction = new ConvertArcAction(workspace, index, arcSegment);
            workspace.RulerCreator.ActionManager.RecordAction(convertArcAction);
        }

        public static void MoveArcMiddlePoint(RulerWorkspace workspace, int index, Point newMiddlePoint,
            Point oldMiddlePoint)
        {
            var moveArcMiddlePointAction =
                new MoveArcMiddlePointAction(workspace, index, newMiddlePoint, oldMiddlePoint);
            workspace.RulerCreator.ActionManager.RecordAction(moveArcMiddlePointAction);
        }

        public static void ConvertLineToArcSegment(RulerWorkspace workspace, int index, ArcSegmentFigure arcSegment)
        {
            var convertLineToArcAction = new ConvertLineToArcAction(workspace, index, arcSegment);

            workspace.RulerCreator.ActionManager.RecordAction(convertLineToArcAction);
        }

        public static void SplitLine(RulerWorkspace workspace, int index, Point newPoint)
        {
            var splitLineAction = new SplitLineAction(workspace, index, newPoint);
            workspace.RulerCreator.ActionManager.RecordAction(splitLineAction);
        }
    }
}