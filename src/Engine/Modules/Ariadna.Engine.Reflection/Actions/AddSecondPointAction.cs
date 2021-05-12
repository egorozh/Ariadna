using AKIM.Undo;
using System.Windows;

namespace AKIM.Engine.TwoD.Reflection
{
    internal class AddSecondPointAction : AbstractAction
    {

        private readonly ReflectionWorkspace _createdFigure;
        private readonly Point _secondPoint;

        public AddSecondPointAction(Point secondPoint, ReflectionWorkspace createdFigure)
        {
            _secondPoint = secondPoint;
            _createdFigure = createdFigure;

            Notice = "Установлена вторая точка";
        }

        protected override void ExecuteCore()
        {
            _createdFigure.CreateSecondPoint(_secondPoint);
        }

        protected override void UnExecuteCore()
        {
            _createdFigure.RemoveSecondPoint();
        }
    }
}
