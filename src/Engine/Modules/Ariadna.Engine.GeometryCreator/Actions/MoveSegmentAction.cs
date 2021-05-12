using System.Windows;
using AKIM.Undo;

namespace Ariadna.Engine.GeometryCreator
{
    internal class MoveSegmentAction : AbstractAction
    {
        private readonly GeometryWorkspace _workspace;
        private readonly int _index;
        private readonly Point _newCenterPoint;
        private readonly Point _oldCenterPoint;

        public MoveSegmentAction(GeometryWorkspace workspace, in int index, Point newCenterPoint, Point oldCenterPoint)
        {
            _workspace = workspace;
            _index = index;
            _newCenterPoint = newCenterPoint;
            _oldCenterPoint = oldCenterPoint;

            Notice = "Перемещение сегмента";
        }

        protected override void ExecuteCore()
        {
            _workspace.MoveSegment(_index, _newCenterPoint);
        }

        protected override void UnExecuteCore()
        {
            _workspace.MoveSegment(_index, _oldCenterPoint);
        }
    }
}