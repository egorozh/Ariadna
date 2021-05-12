using AKIM.Undo;
using System.Windows;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal class AddStartPointAction : AbstractAction
    {
        private readonly RulerWorkspace _createdFigure;
        private readonly Point _startPoint;

        public AddStartPointAction(Point startPoint, RulerWorkspace createdFigure)
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