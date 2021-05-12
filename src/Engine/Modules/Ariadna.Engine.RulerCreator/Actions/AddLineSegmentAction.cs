using System.Windows;
using AKIM.Undo;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal class AddLineSegmentAction : AbstractAction
    {
        private readonly Point _linePoint;
        private readonly RulerWorkspace _workspace;
        private readonly bool _startPointIsSelected;

        public AddLineSegmentAction(Point linePoint, RulerWorkspace workspace, bool startPointIsSelected)
        {
            _linePoint = linePoint;
            _workspace = workspace;
            _startPointIsSelected = startPointIsSelected;

            Notice = "Добавлен сегмент линии";
        }

        protected override void ExecuteCore()
        {
            _workspace.AddLineSegment(_linePoint, _startPointIsSelected);
        }

        protected override void UnExecuteCore()
        {
            _workspace.RemoveLastSegment(_startPointIsSelected);
        }
    }
}