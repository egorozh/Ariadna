using System.Windows;
using AKIM.Undo;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal class MovePointAction : AbstractAction
    {
        private readonly int _index;
        private readonly Point _oldPoint;
        private readonly Point _newPoint;
        private readonly RulerWorkspace _workspace;

        public MovePointAction(int index, Point oldPoint, Point newPoint, RulerWorkspace workspace)
        {
            _index = index;
            _oldPoint = oldPoint;
            _newPoint = newPoint;
            _workspace = workspace;

            Notice = "Перемещение узл. точки";
        }

        protected override void ExecuteCore()
        {
            _workspace.MovePoint(_index, _newPoint);
        }

        protected override void UnExecuteCore()
        {
            _workspace.MovePoint(_index, _oldPoint);
        }
    }
}