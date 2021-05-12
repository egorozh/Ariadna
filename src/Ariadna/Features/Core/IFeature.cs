namespace Ariadna
{
    /// <summary>
    /// Представляет функциональность в основной программе
    /// </summary>
    public interface IFeature
    {
        /// <summary>
        /// Основное приложение
        /// </summary>
        IApp App { get; }

        /// <summary>
        /// Ariadna App   
        /// </summary>
        AriadnaApp AriadnaApp { get;  }
            
        /// <summary>
        /// Id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Вызывается при закрытии окна приложения
        /// </summary>
        /// <returns>Отмена закрытия</returns>
        bool Closing();
    }
}