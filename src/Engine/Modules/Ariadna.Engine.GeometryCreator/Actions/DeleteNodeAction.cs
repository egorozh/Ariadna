using System.Windows;
using AKIM.Undo;

namespace Ariadna.Engine.GeometryCreator
{
    internal sealed class DeleteNodeAction : AbstractAction
    {
        private readonly GeometryWorkspace _workspace;
        private readonly int _index;
        private readonly Point _startPoint;
        private readonly SegmentFigure _segment;
        private readonly SegmentFigure _nextSegment;

        public DeleteNodeAction(GeometryWorkspace workspace, int index, Point startPoint, SegmentFigure segment,
            SegmentFigure nextSegment)
        {
            _workspace = workspace;
            _index = index;
            _startPoint = startPoint;
            _segment = segment;
            _nextSegment = nextSegment;

            Notice = "Удалена узл. точка";
        }

        protected override void ExecuteCore()
        {
            _workspace.DeleteSelectedNode(_index);
        }

        protected override void UnExecuteCore()
        {
            _workspace.UnDeleteSelectedNode(_index, _startPoint, _segment, _nextSegment);
        }
    }
}