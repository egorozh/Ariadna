using System.Windows.Controls;

namespace Ariadna;

/// <summary>
/// Предоставляет коллекцию элементов меню для управления темами приложения
/// </summary>
public interface IThemeAndAccentManager
{
    event EventHandler ThemeChanged;
            
    /// <summary>
    /// Темы
    /// </summary>
    Dictionary<string, string> Themes { get; set; }

    /// <summary>
    /// Основные цвета
    /// </summary>
    Dictionary<string, string> Accents { get; set; }

    /// <summary>
    /// Текущая тема
    /// </summary>
    KeyValuePair<string, string> CurrentTheme { get; set; }

    /// <summary>
    /// Текущий цвет оформления
    /// </summary>
    KeyValuePair<string, string> CurrentAccent { get; set; }

    /// <summary>
    /// Коллекция элементов меню выбора тем
    /// </summary>
    ICollection<MenuItem> ThemesMenuItems { get; }

    /// <summary>
    /// Коллекция элементов меню выбора цветов оформления
    /// </summary>
    ICollection<MenuItem> AccentsMenuItems { get; }
}