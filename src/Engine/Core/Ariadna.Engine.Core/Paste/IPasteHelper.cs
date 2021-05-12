using System;
using System.Collections.Generic;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Позволяет отобразить вставляемые элементы и возвратить измененные геометрии после их перемещения
    /// </summary>
    public interface IPasteHelper : IEngineComponent
    {
        /// <summary>
        /// Событие окончания вставки
        /// </summary>
        event EventHandler<PasteResultEventArgs> PasteEnded;

        /// <summary>
        /// Вставить элементы с данными геометриями
        /// </summary>
        /// <param name="geometries"></param>
        void Paste(List<string> geometries);

        /// <summary>
        /// Отмена вставки
        /// </summary>
        void Cancel();
    }

    public class PasteResultEventArgs : EventArgs
    {
        /// <summary>
        /// Принятие вставки
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Полученные геометрии
        /// </summary>
        public IList<string> Geometries { get; }

        public PasteResultEventArgs(bool success, IList<string> geometries)
        {
            Success = success;
            Geometries = geometries;
        }
    }
}