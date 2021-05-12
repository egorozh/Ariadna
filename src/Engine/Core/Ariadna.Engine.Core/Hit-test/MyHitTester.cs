using System.Collections.Generic;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Кастомная реализация hit-tester'a, работающая на глобальных геометриях фигур
    /// </summary>
    public static class GlobalFigureHitTester
    {
        public static (List<ISelectedFigure2D>, List<ISelectedFigure2D>) HitTest(IFigure2DCollection figures,
            bool isInverseSelection, Geometry intersectGeometry, IntersectSelectionMode mode,
            List<ISelectedFigure2D> initUnselectedFigures)
        {
            var selectedFigures = new List<ISelectedFigure2D>();

            var unSelectedFigures = new List<ISelectedFigure2D>();

            if (initUnselectedFigures != null)
                unSelectedFigures = initUnselectedFigures;

            for (var i = 0; i < figures.Count; i++)
            {
                var figure = figures[i];

                if (!figure.IsShow)
                    continue;

                if (figure is ISelectedFigure2D selectedFigure)
                {
                    if (selectedFigure.IsHitTest(intersectGeometry, mode))
                    {
                        var isSelected = !isInverseSelection || !selectedFigure.IsSelected;
                        Select(isSelected, unSelectedFigures, selectedFigure, selectedFigures);
                    }
                }
            }

            return (selectedFigures, unSelectedFigures);
        }

        private static void Select(bool isSelectedFigure, List<ISelectedFigure2D> unSelectedFigures,
            ISelectedFigure2D selectedFigure, List<ISelectedFigure2D> selectedFigures)
        {
            if (isSelectedFigure)
            {
                if (unSelectedFigures.Contains(selectedFigure))
                    unSelectedFigures.Remove(selectedFigure);

                selectedFigures.Add(selectedFigure);
            }
            else
            {
                if (!unSelectedFigures.Contains(selectedFigure))
                    unSelectedFigures.Add(selectedFigure);
            }
        }
        
        public static ISelectedFigure2D SingleHitTest(IFigure2DCollection figures, Geometry intersectGeometry)
        {
            for (var i = 0; i < figures.Count; i++)
            {
                var figure = figures[i];

                if (!figure.IsShow)
                    continue;

                if (figure is ISelectedFigure2D selectedFigure)
                {
                    if (selectedFigure.IsHitTest(intersectGeometry))
                        return selectedFigure;
                }
            }

            return null;
        }
    }
}