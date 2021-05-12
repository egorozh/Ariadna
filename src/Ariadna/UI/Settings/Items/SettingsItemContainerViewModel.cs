using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Ariadna.Core;

namespace Ariadna
{
    /// <summary>
    /// View-Model элемента настроек
    /// </summary>
    public class SettingsItemContainerViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Элемент выделен
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Элемент развёрнут
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Представление
        /// </summary>
        public virtual FrameworkElement? View => GetElement();

        /// <summary>
        /// Дочерние настройки
        /// </summary>
        public ObservableCollection<SettingsItemContainerViewModel> Children { get; } = new();

        #endregion

        #region Events

        /// <summary>
        /// Происходит при выделении элемента
        /// </summary>
        public event EventHandler? SelectedSetted;

        #endregion

        #region Constructor

        public SettingsItemContainerViewModel(string name)
        {
            Name = name;
            PropertyChanged += SettingsItemContainerViewModel_PropertyChanged;
        }

        #endregion

        #region Private Methods

        private void SettingsItemContainerViewModel_PropertyChanged(object sender,
            PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsSelected):
                {
                    if (IsSelected)
                        SelectedSetted?.Invoke(this, EventArgs.Empty);
                    break;
                }
            }
        }

        private FrameworkElement? GetElement()
        {
            var child = TreeTraverse(Children, item =>
            {
                if (item is SettingsItemViewModel settingsItemViewModel)
                {
                    return settingsItemViewModel.View != null;
                }

                return false;
            });

            return child?.View;
        }

        private static SettingsItemContainerViewModel? TreeTraverse(
            ObservableCollection<SettingsItemContainerViewModel> children,
            Predicate<SettingsItemContainerViewModel> func)
        {
            foreach (var child in children)
            {
                if (func(child))
                    return child;
            }

            foreach (var child in children)
            {
                var res = TreeTraverse(child.Children, func);

                if (res != null)
                {
                    return res;
                }
            }

            return null;
        }

        #endregion
    }
}