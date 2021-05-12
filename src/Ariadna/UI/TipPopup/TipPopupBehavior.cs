using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
    internal class TipPopupBehavior : Behavior<Control>
    {
        private readonly ITipPopupManager _popupManager;
        private readonly TipPopupViewModel _tipPopupViewModel;

        public TipPopupBehavior(ITipPopupManager popupManager, TipPopupViewModel tipPopupViewModel)
        {
            _popupManager = popupManager;
            _tipPopupViewModel = tipPopupViewModel;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_popupManager.IsOpen)
            {
                _popupManager.IsOpen = false;
                _popupManager.IsOpen = true;
                _popupManager.Context = _tipPopupViewModel;
            }
            else
            {
                _popupManager.IsOpen = true;
                _popupManager.Context = _tipPopupViewModel;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}