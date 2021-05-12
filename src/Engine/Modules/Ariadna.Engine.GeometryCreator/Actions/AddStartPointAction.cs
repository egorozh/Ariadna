using System.Windows;
using AKIM.Undo;

namespace Ariadna.Engine.GeometryCreator
{
    internal class AddStartPointAction : AbstractAction
    {
        private readonly GeometryWorkspace _createdFigure;
        private readonly Point _startPoint;

        public AddStartPointAction(Point startPoint, GeometryWorkspace createdFigure)
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