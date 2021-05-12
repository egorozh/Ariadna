using System.Runtime.CompilerServices;
using System.Windows;

namespace AKIM.Engine.TwoD.Reflection
{
    internal static class Actions
    {
        public static void AddStartPoint(ReflectionWorkspace workspace, Point startPoint)
        {
            var addStartPointAction =
                new AddStartPointAction(workspace.ReflectionManager.CoordinateSystem.GetGlobalPoint(startPoint),
                    workspace);

            workspace.ReflectionManager.ActionManager.RecordAction(addStartPointAction);
        }

        public static void AddSecondPoint(ReflectionWorkspace workspace, Point secondPoint)
        {
            var addSecondPointAction =
                new AddSecondPointAction(workspace.ReflectionManager.CoordinateSystem.GetGlobalPoint(secondPoint),
                    workspace);

            workspace.ReflectionManager.ActionManager.RecordAction(addSecondPointAction);
        }
    }
}