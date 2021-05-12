using System.Windows;

namespace Ariadna.Engine.PointCreator
{
    internal static class Actions
    {
        public static void AddStartPoint(PointWorkspace workspace, Point startPoint)
        {
            workspace.CreateStartPoint(workspace.PointCreator.CoordinateSystem.GetGlobalPoint(startPoint));
        }
    }
}