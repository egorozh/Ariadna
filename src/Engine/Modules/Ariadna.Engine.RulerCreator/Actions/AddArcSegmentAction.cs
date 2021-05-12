using AKIM.Undo;
using System.Windows;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal class AddArcSegmentAction : AbstractAction
    {
        private readonly Point _middleArcPoint;
        private readonly Point _endPoint;
        private readonly RulerWorkspace _workspace;
        private readonly bool _startPointIsSelected;

        public AddArcSegmentAction(Point middleArcPoint, Point endPoint, RulerWorkspace workspace,
            bool startPointIsSelected)
        {
            _middleArcPoint = middleArcPoint;
            _endPoint = endPoint;
            _workspace = workspace;
            _startPointIsSelected = startPointIsSelected;

            Notice = "Добавлен сегмент дуги";
        }

        protected override void ExecuteCore()
        {
            _workspace.AddArcSegment(_middleArcPoint, _endPoint, _startPointIsSelected);
        }

        protected override void UnExecuteCore()
        {
            _workspace.RemoveLastSegment(_startPointIsSelected);
        }
    }
}