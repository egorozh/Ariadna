using System;
using System.Windows;

namespace Ariadna
{
    public abstract class ComboboxFeature<T> : InterfaceFeature, IComboboxFeature where T : class
    {
        #region Private Fields

        private T _current;
        private bool _isEnabled;

        #endregion

        #region Public Properties

        public ViewModelItemCollection<T> ItemsCollection { get; } = new ViewModelItemCollection<T>();
        public DataTemplate ItemTemplate { get; set; }

        public T Current
        {
            get => _current;
            set => SetCurrent(value);
        }

        public object SelectedItem
        {
            get => Current;
            set
            {
                _current = (T) value;

                SelectionChanged?.Invoke(this, EventArgs.Empty);

                CurrentChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetIsEnabled(value);
        }

        public IViewModelItemCollection Items => ItemsCollection;

        #endregion

        #region Events

        public virtual string MenuItemHeaderNameProperty => "Name";

        public event EventHandler CurrentChangedEvent;
        public event EventHandler SelectionChanged;
        public event EventHandler IsEnabledChanged;
                
        #endregion

        #region Constructor

        protected ComboboxFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion

        #region Protected Methods

        protected virtual void CurrentChanged()
        {
        }

        #endregion

        #region Private Methods

        private void SetCurrent(T value)
        {
            _current = value;

            CurrentChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void SetIsEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;

            IsEnabledChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}