using System.Windows;
using System.Windows.Media;

namespace Ariadna;

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
    
    DataTemplate GetTemplate();
}