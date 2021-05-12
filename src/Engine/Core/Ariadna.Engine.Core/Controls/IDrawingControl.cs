using System.Windows.Controls;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Контрол, в котором отображаются фигуры, подсказка, сетка и панель помощи
    /// </summary>
    public interface IDrawingControl : IEngineComponent
    {
        ContentControl SubstrateControl { get; }

        /// <summary>
        /// Подложка с сеткой
        /// </summary>
        IGridCanvas GridCanvas { get; }

        ICanvas Canvas { get; }

        void ShowMessage(string message, bool isWarning = false);

        bool Focus();
    }
}   