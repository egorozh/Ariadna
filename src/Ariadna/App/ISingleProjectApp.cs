namespace Ariadna
{
    public interface ISingleProjectApp : IApp
    {
        /// <summary>
        /// Основное окно приложения
        /// </summary>
        object View { get; set; }
    }
}