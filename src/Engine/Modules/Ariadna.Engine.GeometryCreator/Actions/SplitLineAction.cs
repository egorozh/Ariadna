using System.Windows;
using AKIM.Undo;

namespace Ariadna.Engine.GeometryCreator
{
    internal class SplitLineAction : AbstractAction
    {
        private readonly GeometryWorkspace _workspace;
        private readonly int _index;
        private readonly Point _newPoint;

        public SplitLineAction(GeometryWorkspace workspace, int index, Point newPoint)
        {
            _workspace = workspace;
            _index = index;
            _newPoint = newPoint;

            Notice = "Разделение линии";
        }

        protected override void ExecuteCore()
        {
            _workspace.SplitLineSegment(_index, _newPoint);
        }

        protected override void UnExecuteCore()
        {
            _workspace.UnSplitLineSegment(_index);
        }
    }
}