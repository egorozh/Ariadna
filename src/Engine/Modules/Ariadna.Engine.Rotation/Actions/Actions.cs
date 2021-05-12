using System.Runtime.CompilerServices;
using System.Windows;

namespace AKIM.Engine.TwoD.Rotation
{
    internal static class Actions
    {
        public static void AddStartPoint(RotationWorkspace workspace, Point startPoint)
        {
            var addStartPointAction =
                new AddStartPointAction(workspace.RotationManager.CoordinateSystem.GetGlobalPoint(startPoint),
                    workspace);

            workspace.RotationManager.ActionManager.RecordAction(addStartPointAction);
        }

        public static void AddSecondPoint(RotationWorkspace workspace, Point secondPoint)
        {
            var addSecondPointAction =
                new AddSecondPointAction(workspace.RotationManager.CoordinateSystem.GetGlobalPoint(secondPoint),
                    workspace);

            workspace.RotationManager.ActionManager.RecordAction(addSecondPointAction);
        }
    }
}