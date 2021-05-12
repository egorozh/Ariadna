using System.Windows;
using System.Windows.Media;
using Ariadna.Core;
using Prism.Commands;

namespace Ariadna
{
    /// <summary>
    /// Класс модели документа
    /// </summary>
    public abstract class PaneViewModel : BaseViewModel, IPaneViewModel
    {
        #region Public Properties
        
        /// <summary>
        /// Заголовок
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Иконка
        /// </summary>
        public ImageSource IconSource { get; set; }

        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public string ContentId { get; set; }

        /// <summary>
        /// Документ выделен в данный момент
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Документ активен в данный момент
        /// </summary>
        public bool IsActive { get; set; }

        #endregion

        #region Commands

        public DelegateCommand CloseCommand { get; }

        #endregion

        protected PaneViewModel()
        {
            CloseCommand = new DelegateCommand(Close);
        }

        public virtual void Close()
        {
        }

        public abstract DataTemplate GetTemplate();
    }

    public interface IPaneViewModel
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Иконка
        /// </summary>
        ImageSource IconSource { get; set; }

        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        string ContentId { get; set; }

        /// <summary>
        /// Документ выделен в данный момент
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Документ активен в данный момент
        /// </summary>
        bool IsActive { get; set; }

        DelegateCommand CloseCommand { get; }

        DataTemplate GetTemplate();
    }
}