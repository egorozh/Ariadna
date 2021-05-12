using AKIM.Undo;

namespace Ariadna.Engine.GeometryCreator
{
    internal class ConvertArcAction : AbstractAction
    {
        private readonly GeometryWorkspace _workspace;
        private readonly int _index;
        private readonly ArcSegmentFigure _arcSegment;

        public ConvertArcAction(GeometryWorkspace workspace, int index, ArcSegmentFigure arcSegment)
        {
            _workspace = workspace;
            _index = index;
            _arcSegment = arcSegment;

            Notice = "Дуга -> линия";
        }

        protected override void ExecuteCore()
        {
            _workspace.ConvertArcToLine(_index, _arcSegment);
        }

        protected override void UnExecuteCore()
        {
            _workspace.ConvertLineToArc(_index, _arcSegment);
        }
    }
}