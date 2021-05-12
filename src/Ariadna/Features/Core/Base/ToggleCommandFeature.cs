using System;

namespace Ariadna
{
    /// <summary>
    /// Базовый класс функциональности в виде команды-переключателя
    /// </summary>
    public abstract class ToggleCommandFeature : CommandFeature, IToggleCommandFeature
    {
        #region Private Fields

        private bool _isPressed;

        #endregion

        #region Public Properties

        public bool IsPressed
        {
            get => _isPressed;
            set => SetIsPressed(value);
        }

        #endregion

        #region Events

        public event EventHandler IsPressedChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор
        /// </summary>
        protected ToggleCommandFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion

        #region Protected Methods

        protected abstract bool Unchecked();
            
        protected abstract void Checked();

        #endregion

        #region Internal Methods

        private void SetIsPressed(bool value)
        {
            if (value)
                Checked();
            else
            {
                var isNotUnchecked = Unchecked();

                if (isNotUnchecked)
                {
                    return;
                }
            }

            _isPressed = value;

            IsPressedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}