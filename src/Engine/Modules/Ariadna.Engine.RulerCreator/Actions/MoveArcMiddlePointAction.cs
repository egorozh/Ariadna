using System.Windows;
using AKIM.Undo;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal class MoveArcMiddlePointAction : AbstractAction
    {
        private readonly RulerWorkspace _workspace;
        private readonly int _index;
        private readonly Point _newMiddlePoint;
        private readonly Point _oldMiddlePoint;

        public MoveArcMiddlePointAction(RulerWorkspace workspace,
            int index, Point newMiddlePoint, Point oldMiddlePoint)
        {
            _workspace = workspace;
            _index = index;
            _newMiddlePoint = newMiddlePoint;
            _oldMiddlePoint = oldMiddlePoint;

            Notice = "Изменение дуги";
        }

        protected override void ExecuteCore()
        {
            _workspace.MoveMiddleArcPoint(_index, _newMiddlePoint);
        }

        protected override void UnExecuteCore()
        {
            _workspace.MoveMiddleArcPoint(_index, _oldMiddlePoint);
        }
    }
}