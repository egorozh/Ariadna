using Serilog;

namespace Ariadna
{
    /// <summary>
    /// Базовый класс функциональности
    /// </summary>
    public abstract class Feature : IFeature
    {   
        #region Private Fields

        private string? _id;

        #endregion

        #region Public Properties

        /// <summary>
        /// Основное приложение 
        /// </summary>
        public static IApp AppInstance { get; private set; }

        public ILogger Logger { get; }

        public IApp App => AppInstance;
        public AriadnaApp AriadnaApp { get; }

        /// <summary>
        /// Id  
        /// </summary>
        public string Id => _id ??= CreateId();

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор
        /// </summary>
        protected Feature(AriadnaApp ariadnaApp)
        {
            AppInstance = ariadnaApp.App;
            AriadnaApp = ariadnaApp;
            Logger = ariadnaApp.Logger;
        }

        #endregion

        /// <summary>
        /// Вызывается при закрытии окна приложения
        /// </summary>
        /// <returns>Отмена закрытия</returns>
        public virtual bool Closing()
        {
            // Не отменять закрытие
            return false;
        }

        /// <summary>
        /// Возвращает уникальный идентификатор функциональности
        /// </summary>
        /// <remarks>Функция должна возвращать один и тот же ID при каждом вызове</remarks>
        /// <returns>Уникальный идентификатор функциональности</returns>
        protected abstract string CreateId();

      
    }

    
}