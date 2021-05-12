using System.Collections.Generic;

namespace Ariadna.Engine.Core
{
    public interface IGroupFigure : ISelectedFigure2D
    {
        IReadOnlyList<IFigure2D> Figures { get; }
    }
}