using Ariadna.Core;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace Ariadna;

internal class SettingsDialogViewModel : BaseViewModel
{
    #region Private Fields

    private readonly SettingsManager _settingsManager;

    #endregion

    #region Public Properties

    /// <summary>
    /// Элементы настроек
    /// </summary>
    public ObservableCollection<SettingsItemContainerViewModel> Items { get; set; } = new();

    /// <summary>
    /// Выделенный элемент
    /// </summary>
    public SettingsItemContainerViewModel? SelectedItem { get; set; }

    #endregion

    #region Commands

    public DelegateCommand OkCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand AcceptCommand { get; }

    #endregion

    #region Constructor

    public SettingsDialogViewModel(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        OkCommand = new DelegateCommand(Ok);
        CancelCommand = new DelegateCommand(Cancel);
        AcceptCommand = new DelegateCommand(Accept, CanAccept);
    }

    #endregion

    #region Public Methods

    public void Clear()
    {
        Items.Clear();
        SelectedItem = null;
    }

    public void InitElements(List<UiSettingsItem> uiSettingsItems, IEnumerable<ISettings> settings)
    {
        Clear();

        foreach (var setting in settings)
        {
            setting.Init();

            var item = new SettingsItemViewModel(setting, setting.ToString());
            item.HasChangesChanged += HasChangesChanged;
            item.SelectedSetted += Item_SelectedSetted;

            Items.Add(item);
        }

        SelectedItem = Items.FirstOrDefault();
    }

    #endregion

    #region Command Methods

    private bool CanAccept()
    {
        var changedItem = TreeTraverse(Items, item =>
        {
            if (item is SettingsItemViewModel settingsVm)
                return settingsVm.HasChanges;

            return false;
        });

        return changedItem != null;
    }

    private void Accept()
    {
        TreeTraverse(Items, item =>
        {
            if (item is SettingsItemViewModel settingsVm)
            {
                if (settingsVm.HasChanges)
                    settingsVm.Accept();
            }
        });
    }

    private void Cancel()
    {
        TreeTraverse(Items, item =>
        {
            if (item is SettingsItemViewModel settingsVm)
            {
                settingsVm.Cancelled();
            }
        });

        _settingsManager.Close();
    }

    private void Ok()
    {
        if (CanAccept())
            Accept();

        _settingsManager.Close();
    }

    #endregion

    #region Private Methods

    private void Item_SelectedSetted(object? sender, EventArgs e)
    {
        SelectedItem = (SettingsItemContainerViewModel?) sender;
    }

    private void HasChangesChanged(object? sender, EventArgs e)
    {
        AcceptCommand.RaiseCanExecuteChanged();
    }

    private static SettingsItemContainerViewModel? TreeTraverse(
        IEnumerable<SettingsItemContainerViewModel> items,
        Predicate<SettingsItemContainerViewModel> predicate)
    {
        foreach (var item in items)
        {
            if (predicate.Invoke(item))
                return item;

            var res = TreeTraverse(item.Children, predicate);

            if (res != null)
                return res;
        }

        return null;
    }

    private static void TreeTraverse(IEnumerable<SettingsItemContainerViewModel> items,
        Action<SettingsItemContainerViewModel> action)
    {
        foreach (var item in items)
        {
            action.Invoke(item);

            TreeTraverse(item.Children, action);
        }
    }

    #endregion
}