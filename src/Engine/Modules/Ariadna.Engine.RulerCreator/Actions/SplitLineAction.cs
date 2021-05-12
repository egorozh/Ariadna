using System.Windows;
using AKIM.Undo;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal class SplitLineAction : AbstractAction
    {
        private readonly RulerWorkspace _workspace;
        private readonly int _index;
        private readonly Point _newPoint;

        public SplitLineAction(RulerWorkspace workspace, int index, Point newPoint)
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