using AKIM.Undo;
using System.Windows;

namespace AKIM.Engine.TwoD.Rotation
{
    internal class AddStartPointAction : AbstractAction
    {
        private readonly RotationWorkspace _createdFigure;
        private readonly Point _startPoint;

        public AddStartPointAction(Point startPoint, RotationWorkspace createdFigure)
        {
            _startPoint = startPoint;
            _createdFigure = createdFigure;

            Notice = "Установлена стартовая точка";
        }

        protected override void ExecuteCore()
        {
            _createdFigure.CreateStartPoint(_startPoint);
        }

        protected override void UnExecuteCore()
        {
            _createdFigure.RemoveStartPoint();
        }
    }
}