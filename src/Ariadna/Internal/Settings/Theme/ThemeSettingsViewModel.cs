using Ariadna.Core;
using System.Collections.Generic;

namespace Ariadna.Settings.Theme
{
    internal class ThemeSettingsViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly IThemeAndAccentManager _themeManager;
        private readonly ISettings _themeSettings;

        #endregion

        #region Public Properties

        /// <summary>
        /// Текущая тема
        /// </summary>
        public KeyValuePair<string, string> CurrentTheme { get; set; }

        /// <summary>
        /// Текущий цвет оформления
        /// </summary>
        public KeyValuePair<string, string> CurrentAccent { get; set; }

        /// <summary>
        /// Темы
        /// </summary>
        public Dictionary<string, string> Themes { get; set; }

        /// <summary>
        /// Основные цвета
        /// </summary>
        public Dictionary<string, string> Accents { get; set; }

        #endregion

        #region Constructor

        public ThemeSettingsViewModel(IThemeAndAccentManager themeManager, ISettings themeSettings)
        {
            _themeManager = themeManager;
            _themeSettings = themeSettings;

            CurrentTheme = _themeManager.CurrentTheme;
            CurrentAccent = _themeManager.CurrentAccent;
            Themes = _themeManager.Themes;
            Accents = _themeManager.Accents;

            PropertyChanged += ThemeSettingsViewModel_PropertyChanged;
            _themeManager.ThemeChanged += _themeManager_IsThemeChanged;
        }

        private void _themeManager_IsThemeChanged(object sender, System.EventArgs e)
        {
            CurrentTheme = _themeManager.CurrentTheme;
            CurrentAccent = _themeManager.CurrentAccent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Принять изменения
        /// </summary>
        public void Accept()
        {
            _themeManager.CurrentAccent = CurrentAccent;
            _themeManager.CurrentTheme = CurrentTheme;
        }

        public void Cancel()
        {
        }

        #endregion

        #region Private Methods

        private void ThemeSettingsViewModel_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!CurrentAccent.Equals(_themeManager.CurrentAccent) ||
                !CurrentTheme.Equals(_themeManager.CurrentTheme))
                _themeSettings.HasChanges = true;
        }

        #endregion
    }
}