using System;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    /// <summary>
    /// Рабочее пространство, где происходит редактирование геометрии
    /// </summary>
    internal interface IRulerWorkspace : IDisposable
    {
        bool IsClosed { get; }

        event EventHandler SelectedChanged;
        
        void SetCreationMode(CreationMode mode);

        bool CanContinue();
        void Continue();

        void SetIsClosed(bool isClosed);
      
        void DeleteSelectedPoint();
        bool CanDeleteSelectedPoint();

        void ConvertSelectedArcSegment();
        bool CanConvertSelectedArcSegment();
    }
}