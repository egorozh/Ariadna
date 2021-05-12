using System;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.GeometryCreator
{
    /// <summary>
    /// Рабочее пространство, где происходит редактирование геометрии
    /// </summary>
    internal interface IGeometryWorkspace : IDisposable
    {
        bool IsClosed { get; }

        event EventHandler SelectedChanged;
            
        PathGeometry GetData();

        void SetCreationMode(CreationMode mode);

        bool CanContinue();
        void Continue();


        void SetIsClosed(bool isClosed);
        bool CanAccessCreating();

        void DeleteSelectedPoint();
        bool CanDeleteSelectedPoint();

        void ConvertSelectedArcSegment();
        bool CanConvertSelectedArcSegment();
    }
}