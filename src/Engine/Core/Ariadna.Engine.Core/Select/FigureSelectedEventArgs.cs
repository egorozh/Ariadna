using System;
using System.Collections.Generic;

namespace Ariadna.Engine.Core
{
    public class FigureSelectedEventArgs : EventArgs
    {
        public List<ISelectedFigure2D>? SelectedFigures { get; }
        public List<ISelectedFigure2D>? UnSelectedFigures { get; }
        public object? SyncToken { get; }

        public FigureSelectedEventArgs(List<ISelectedFigure2D>? selectedFigures,
            List<ISelectedFigure2D>? unSelectedFigures, object? syncToken = null)
        {
            SelectedFigures = selectedFigures;
            UnSelectedFigures = unSelectedFigures;
            SyncToken = syncToken;
        }
    }
}