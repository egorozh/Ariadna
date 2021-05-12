namespace Ariadna
{
    public class DefaultRibbonProperties
    {   
        #region Public Properties

        public string Name { get; set; } = "Команда 1";

        public RibbonItemSize Size { get; set; }
        
        public string Description { get; set; } = "Описание не задано";

        public string DisableReason { get; set; } = "Причина неактивности комманды не указана";

        #endregion
    }

    /// <summary>
    /// Размер кнопки в ленте
    /// </summary>
    public enum RibbonItemSize
    {
        Large,

        Middle,

        Small
    }
}