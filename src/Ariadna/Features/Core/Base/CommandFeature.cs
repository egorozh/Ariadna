#if RELEASE
using System;
#endif
using System.Windows;
using System.Windows.Input;
using Prism.Commands;

namespace Ariadna
{
    /// <summary>
    /// Базовый класс функциональности в виде обычной команды 
    /// </summary>
    public abstract class CommandFeature : InterfaceFeature, ICommandFeature
    {
        #region Private Fields

        private readonly DelegateCommand _command;

        private KeyBinding _keyBinding;

        private static bool _isGlobalLockCommand = false;

        #endregion

        #region Protected Fields

        /// <summary>
        /// Ручная установка поля SetIsGlobalLockCommand в методе OnExecute
        /// </summary>
        protected bool IsManualSetGlobalLockCommand = false;

        #endregion

        #region Protected Properties

        protected static bool IsGlobalLockCommand
        {
            get => _isGlobalLockCommand;
            set => SetIsGlobalLockCommand(value);
        }

        #endregion

        #region Public Properties

        public ICommand Command => _command;

        public KeyBinding KeyBinding => _keyBinding ??= CreateDefaultKeyBinding();

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор
        /// </summary>
        protected CommandFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
            _command = new DelegateCommand(OnExecute, OnCanExecute);
        }

        #endregion

        #region Abstract Methods

        protected virtual bool CanExecute() => true;

        protected virtual void Execute()
        {
        }

        protected virtual KeyBinding CreateDefaultKeyBinding() => null;

        public virtual FrameworkElement CreateDefaultIcon() => null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Происходит при обновлении программы
        /// </summary>
        public virtual void Update()
        {
            _command?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Происходит при загрузке программы
        /// </summary>
        public virtual void Load()
        {
        }

        #endregion

        #region Private Methods

        private static void SetIsGlobalLockCommand(bool value)
        {
            _isGlobalLockCommand = value;

            //foreach (var command in AppInstance.GetCurrentModuleFeatures().OfType<ICommandFeature>())
            //    command.Update();
        }

        private void OnExecute()
        {
            if (IsManualSetGlobalLockCommand)
            {
#if DEBUG
                Execute();
                return;
#endif
#if RELEASE
                try
                {
                    Execute();
                    return;
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                    return;
                }
#endif
            }

            IsGlobalLockCommand = true;

            Update();
#if DEBUG
            Execute();
#endif
#if RELEASE
            try
            {
                Execute();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
#endif
            IsGlobalLockCommand = false;

            Update();
        }

        private bool OnCanExecute()
        {
            if (IsGlobalLockCommand)
                return false;

#if DEBUG
            return CanExecute();
#endif
#if RELEASE
            try
            {
                return CanExecute();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
#endif
        }

        #endregion
    }
}