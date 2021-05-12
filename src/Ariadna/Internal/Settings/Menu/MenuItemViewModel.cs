using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Ariadna.Core;

namespace Ariadna
{
    internal class MenuItemViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly MenuSettingsViewModel _menuSettingsViewModel;

        #endregion

        #region Public Properties

        public MenuItemViewModel Parent { get; }

        public string Header { get; set; }

        public ObservableCollection<MenuItemViewModel> Children { get; } =
            new ObservableCollection<MenuItemViewModel>();

        public bool IsVisibleIcon => Icon != null;

        public IFeature Feature { get; set; }

        public string FeatureName => Feature?.ToString();

        public FrameworkElement Icon { get; }

        public FrameworkElement IconLarge { get; }

        /// <summary>
        /// Элемент выделен
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Элемент развёрнут
        /// </summary>
        public bool IsExpanded { get; set; }

        public bool IsReadonlyItem { get; set; }

        #endregion

        #region Constructors

        public MenuItemViewModel(UiMenuItem uiMenuItem, MenuSettingsViewModel menuSettingsViewModel, AriadnaApp akimApp)
        {
            _menuSettingsViewModel = menuSettingsViewModel;
            Header = uiMenuItem.Header;
            
            Feature = akimApp.Features.FirstOrDefault(f => f.Id == uiMenuItem.Id);

            if (Feature is ICommandFeature commandFeature)
            {
                Icon = commandFeature.CreateDefaultIcon();
                IconLarge = commandFeature.CreateDefaultIcon();
            }

            foreach (var item in uiMenuItem.Children)
            {
                var menuItemVm = item.Header switch
                {
                    "$Separator" => new SeparatorItemVm(item, menuSettingsViewModel, this, akimApp),
                    _ => new MenuItemViewModel(item, menuSettingsViewModel, this, akimApp)
                };

                menuItemVm.Init();

                Children.Add(menuItemVm);
            }
        }

        public MenuItemViewModel(UiMenuItem uiMenuItem, MenuSettingsViewModel menuSettingsViewModel,
            MenuItemViewModel parent, AriadnaApp akimApp) : this(uiMenuItem, menuSettingsViewModel, akimApp)
        {
            Parent = parent;
        }

        #endregion

        public void Init()
        {
            PropertyChanged += MenuItemViewModel_PropertyChanged;
        }

        #region Private Methods

        private void MenuItemViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSelected))
            {
                if (IsSelected)
                    _menuSettingsViewModel.SelectedItem = this;
            }
            else if (e.PropertyName != nameof(IsExpanded))
            {
                _menuSettingsViewModel.ItemChanged();
            }
        }

        #endregion
    }

    internal class SeparatorItemVm : MenuItemViewModel
    {
        public SeparatorItemVm(UiMenuItem uiMenuItem, MenuSettingsViewModel menuSettingsViewModel,
            MenuItemViewModel parent, AriadnaApp akimApp) : base(uiMenuItem,
            menuSettingsViewModel, parent, akimApp)
        {
            IsReadonlyItem = true;
            Header = "Разделитель";
        }
    }

    internal class RootItemVm : MenuItemViewModel
    {
        public RootItemVm(UiMenuItem uiMenuItem, MenuSettingsViewModel menuSettingsViewModel,
            MenuItemViewModel parent, AriadnaApp akimApp) : base(uiMenuItem,
            menuSettingsViewModel, parent, akimApp)
        {
            IsReadonlyItem = true;
            Header = "Меню";
        }
    }
}