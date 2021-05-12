namespace Ariadna
{
    /// <summary>
    /// Базовый класс функциональности с определенным поведением (без указания в интерфейсе)
    /// </summary>
    public abstract class FuncFeature : Feature, IFuncFeature
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        protected FuncFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }
    }
}