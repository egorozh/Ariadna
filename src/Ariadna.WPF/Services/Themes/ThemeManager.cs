using Ariadna.Core;
using ControlzEx.Theming;
using Prism.Commands;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ariadna;

/// <summary>
/// Класс, предоставляющий элементы меню для управления темами и цветовой схемой
/// </summary>
internal class ThemeManager : BaseViewModel, IThemeAndAccentManager
{
    #region Private Fields

    private readonly IUiOptions _options;
    private readonly ICommand _setThemeCommand;
    private readonly ICommand _setAccentCommand;
    private readonly IReadOnlyList<ICustomThemeManager> _customThemeManagers;

    #endregion

    #region Public Properties

    /// <summary>
    /// Темы
    /// </summary>
    public Dictionary<string, string> Themes { get; set; } = new()
    {
        {"Светлая", "Light"},
        {"Тёмная", "Dark"},
    };

    /// <summary>
    /// Основные цвета
    /// </summary>
    public Dictionary<string, string> Accents { get; set; } = new()
    {
        {"Сталь", "#CC647687|Steel"},
        {"Оливковая", "#CC6D8764|Olive"},
        {"Бежевая", "#CC87794E|Taupe"},
        {"Коричневая", "#CC825A2C|Brown"},
        {"Сиенна", "#CCA0522D|Sienna"},

        {"Красная", "#CCE51400|Red"},
        {"Бордовая", "#CCA20025|Crimson"},
        {"Малиновая", "#CCD80073|Magenta"},
        {"Розовая", "#CCF472D0|Pink"},

        {"Фиолетоая", "#CCAA00FF|Violet"},
        {"Индиго", "#CC6A00FF|Indigo"},
        {"Сиреневая", "#CC6459DF|Purple"},

        {"Кобальт", "#CC0050EF|Cobalt"},
        {"Голубая", "#CC119EDA|Blue"},
        {"Бирюзовая", "#CC00ABA9|Teal"},

        {"Изумрудная", "#CC008A00|Emerald"},
        {"Зелёная", "#CC60A917|Green"},
        {"Лайм", "#CCA4C400|Lime"},
        {"Жёлтая", "#CCFEDE06|Yellow"},
        {"Янтарная", "#CCF0A30A|Amber"},
        {"Оранжевая", "#CCFA6800|Orange"},
    };

    /// <summary>
    /// Текущая тема
    /// </summary>
    public KeyValuePair<string, string> CurrentTheme { get; set; }

    /// <summary>
    /// Текущий цвет оформления
    /// </summary>
    public KeyValuePair<string, string> CurrentAccent { get; set; }

    /// <summary>
    /// Коллекция элементов меню выбора тем
    /// </summary>
    public ICollection<MenuItem> ThemesMenuItems { get; }

    /// <summary>
    /// Коллекция элементов меню выбора цветов оформления
    /// </summary>
    public ICollection<MenuItem> AccentsMenuItems { get; }

    #endregion

    #region Events

    public event EventHandler ThemeChanged;

    #endregion

    #region Constructor

    /// <summary>
    /// Дефолтный конструктор
    /// </summary>
    public ThemeManager(IUiOptions options, IReadOnlyList<ICustomThemeManager> customThemes)
    {
        _options = options;
        _customThemeManagers = customThemes;
        _setThemeCommand = new DelegateCommand<object>(SetTheme);
        _setAccentCommand = new DelegateCommand<object>(SetAccent);

        var theme = options.Theme.Theme ?? "Тёмная";
        var accent = options.Theme.Accent ?? "Голубая";

        CurrentTheme = GetKeyValuePair(Themes, theme);
        CurrentAccent = GetKeyValuePair(Accents, accent);

        ThemesMenuItems = GetThemeMenuItems();
        AccentsMenuItems = GetAccentMenuItems();

        SetTheme(CurrentTheme);
        SetAccent(CurrentAccent);

        PropertyChanged += ThemeAndAccentManager_PropertyChanged;
        ControlzEx.Theming.ThemeManager.Current.ThemeChanged += ThemeManager_IsThemeChanged;
    }

    #endregion

    #region Private Methods

    private void OnAccentChanged(object oldvalue, KeyValuePair<string, string> newvalue)
    {
        SetAccent(newvalue);
    }

    private void OnThemeChanged(object oldvalue, KeyValuePair<string, string> newvalue)
    {
        SetTheme(newvalue);
    }

    private void SetAccent(object nameAccent)
    {
        CurrentAccent = (KeyValuePair<string, string>) nameAccent;

        _options.Theme.Accent = CurrentAccent.Key;

        CheckMenuItem((KeyValuePair<string, string>) nameAccent, AccentsMenuItems);
        ControlzEx.Theming.ThemeManager.Current.ChangeTheme(Application.Current, $"{GetTheme()}.{GetAccent()}");
    }

    private void SetTheme(object nameTheme)
    {
        CurrentTheme = (KeyValuePair<string, string>) nameTheme;

        _options.Theme.Theme = CurrentTheme.Key;

        CheckMenuItem((KeyValuePair<string, string>) nameTheme, ThemesMenuItems);
        ControlzEx.Theming.ThemeManager.Current.ChangeTheme(Application.Current, $"{GetTheme()}.{GetAccent()}");

        foreach (var themeManager in _customThemeManagers)
        {
            themeManager.SetTheme(this);
        }
       
    }

    private static void CheckMenuItem(KeyValuePair<string, string> name, ICollection<MenuItem> menuItems)
    {
        foreach (var menuItem in menuItems)
            menuItem.IsChecked = name.Equals(menuItem.CommandParameter);
    }

    private ICollection<MenuItem> GetThemeMenuItems()
    {
        var themeMenuItems = new List<MenuItem>();

        foreach (var theme in Themes)
        {
            themeMenuItems.Add(new MenuItem
            {
                Header = theme.Key,
                IsCheckable = true,
                IsChecked = false,
                Command = _setThemeCommand,
                CommandParameter = theme,
            });
        }

        return themeMenuItems;
    }

    private ICollection<MenuItem> GetAccentMenuItems()
    {
        var accentMenuItems = new List<MenuItem>();

        foreach (var accent in Accents)
        {
            var colorValueAndCommandValue = accent.Value
                .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

            accentMenuItems.Add(new MenuItem
            {
                Header = accent.Key,
                Icon = GetAccentEllipse(colorValueAndCommandValue[0]),
                IsCheckable = true,
                IsChecked = false,
                Command = _setAccentCommand,
                CommandParameter = accent
            });
        }

        return accentMenuItems;
    }

    private static Ellipse GetAccentEllipse(string color)
    {
        //#CCA0522D
        var ellipse = new Ellipse
            {Width = 10, Height = 10, Fill = new SolidColorBrush(GetColorFromString(color))};
        return ellipse;
    }

    private static Color GetColorFromString(string colorString)
    {
        //#CCA0522D
        var alpha = byte.Parse(colorString.Substring(1, 2), NumberStyles.AllowHexSpecifier);
        var r = byte.Parse(colorString.Substring(3, 2), NumberStyles.AllowHexSpecifier);
        var g = byte.Parse(colorString.Substring(5, 2), NumberStyles.AllowHexSpecifier);
        var b = byte.Parse(colorString.Substring(7, 2), NumberStyles.AllowHexSpecifier);
        return Color.FromArgb(alpha, r, g, b);
    }

    private string GetTheme()
    {
        return CurrentTheme.Value;
    }

    private string GetAccent()
    {
        var colorValueAndCommandValue = CurrentAccent.Value
            .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

        return colorValueAndCommandValue[1];
    }

    private void ThemeManager_IsThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ThemeAndAccentManager_PropertyChanged(object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e is not ExPropertyChangedEventArgs args)
            return;

        switch (e.PropertyName)
        {
            case nameof(CurrentTheme):
                OnThemeChanged(args.OldValue, (KeyValuePair<string, string>) args.NewValue);
                break;
            case nameof(CurrentAccent):
                OnAccentChanged(args.OldValue, (KeyValuePair<string, string>) args.NewValue);
                break;
        }
    }

    private static KeyValuePair<string, string> GetKeyValuePair(Dictionary<string, string> dictionary, string key)
    {
        return new KeyValuePair<string, string>(key, dictionary[key]);
    }

    #endregion
}