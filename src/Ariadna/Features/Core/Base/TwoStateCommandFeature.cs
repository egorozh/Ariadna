using System;
using System.Windows;

namespace Ariadna
{
    public abstract class TwoStateCommandFeature : CommandFeature, ITwoStateCommandFeature
    {
        #region Private Fields

        private DefaultMenuProperties _alternativeMenuProperties;
        private DefaultRibbonProperties _alternativeRibbonProperties;
        private State _state;

        #endregion

        #region Public Properties

        public State State
        {
            get => _state;
            set => SetState(value);
        }

        #endregion

        public event EventHandler StateChanged;

        #region Constructor

        protected TwoStateCommandFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion

        public DefaultMenuProperties AlternativeMenuProperties =>
            _alternativeMenuProperties ??= CreateAlternativeMenuProperties();

        public DefaultRibbonProperties AlternativeRibbonProperties =>
            _alternativeRibbonProperties ??= CreateAlternativeRibbonProperties();

        protected virtual DefaultRibbonProperties CreateAlternativeRibbonProperties() => null;

        protected virtual DefaultMenuProperties CreateAlternativeMenuProperties() => null;

        public virtual FrameworkElement CreateAlternativeIcon() => null;

        private void SetState(State value)
        {
            _state = value;

            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}