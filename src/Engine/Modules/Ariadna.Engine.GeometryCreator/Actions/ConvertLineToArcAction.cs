using AKIM.Undo;

namespace Ariadna.Engine.GeometryCreator
{
    internal class ConvertLineToArcAction : AbstractAction
    {
        private readonly GeometryWorkspace _workspace;
        private readonly int _index;
        private readonly ArcSegmentFigure _arcSegment;

        public ConvertLineToArcAction(GeometryWorkspace workspace, int index, ArcSegmentFigure arcSegment)
        {
            _workspace = workspace;
            _index = index;
            _arcSegment = arcSegment;

            Notice = "Линия -> дуга";
        }

        protected override void ExecuteCore()
        {
            _workspace.ConvertLineToArc(_index, _arcSegment);
        }

        protected override void UnExecuteCore()
        {
            _workspace.ConvertArcToLine(_index, _arcSegment);
        }
    }
}