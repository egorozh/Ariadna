using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Ariadna;

public class ComboBoxMenuItemBehavior : Behavior<ItemsControl>
{
    #region Private Fields

    private readonly IComboboxFeature _comboboxFeature;
    private readonly int _firstIndex;

    private readonly List<MenuItem> _items = new();
    private bool _lock;

    #endregion

    #region Constructor

    public ComboBoxMenuItemBehavior(IComboboxFeature comboboxFeature, int firstIndex)
    {
        _comboboxFeature = comboboxFeature;
        _firstIndex = firstIndex;
    }

    #endregion

    #region Protected Methods

    protected override void OnAttached()
    {
        InitItems();

        _comboboxFeature.IsEnabledChanged += ComboboxFeature_IsEnabledChanged;
        _comboboxFeature.CurrentChanged += ComboboxFeature_CurrentChangedEvent;

        if (_comboboxFeature.Items is INotifyCollectionChanged notifyCollection)
            notifyCollection.CollectionChanged += Items_CollectionChanged;
    }

    protected override void OnDetaching()
    {
        _comboboxFeature.IsEnabledChanged -= ComboboxFeature_IsEnabledChanged;
        _comboboxFeature.CurrentChanged -= ComboboxFeature_CurrentChangedEvent;

        if (_comboboxFeature.Items is INotifyCollectionChanged notifyCollection)
            notifyCollection.CollectionChanged -= Items_CollectionChanged;
    }

    #endregion

    private void InitItems()
    {
        var index = _firstIndex;

        foreach (var item in _comboboxFeature.Items)
        {
            AddItem(item, index);

            index++;
        }
    }

    private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && !_lock)
        {
            _lock = true;

            menuItem.IsChecked = true;

            _lock = false;
        }
    }

    private void MenuItem_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && !_lock)
        {
            var vm = menuItem.DataContext;

            _comboboxFeature.CurrentChanged -= ComboboxFeature_CurrentChangedEvent;

            _comboboxFeature.CurrentItem = vm;

            _lock = true;

            foreach (var item in _items)
            {
                item.Unchecked -= MenuItem_Unchecked;

                item.IsChecked = item.DataContext == _comboboxFeature.CurrentItem;

                item.Unchecked += MenuItem_Unchecked;
            }

            _lock = false;

            _comboboxFeature.CurrentChanged += ComboboxFeature_CurrentChangedEvent;
        }
    }

    private void ComboboxFeature_SelectionChanged(object? sender, EventArgs e)
    {
        _lock = true;

        foreach (var menuItem in _items)
            menuItem.IsChecked = menuItem.DataContext == _comboboxFeature.CurrentItem;

        _lock = false;
    }

    private void ComboboxFeature_CurrentChangedEvent(object? sender, EventArgs e)
    {
        _lock = true;

        foreach (var menuItem in _items)
            menuItem.IsChecked = menuItem.DataContext == _comboboxFeature.CurrentItem;

        _lock = false;
    }

    private void ComboboxFeature_IsEnabledChanged(object? sender, EventArgs e)
    {
        foreach (var item in _items)
            item.IsEnabled = _comboboxFeature.IsEnabled;
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var addedItem = e.NewItems[0];

            var index = _firstIndex + _items.Count;
            AddItem(addedItem, index);
        }

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            var removedItem = e.OldItems[0];

            var menuItem = _items.FirstOrDefault(m => m.DataContext == removedItem);

            if (menuItem != null)
            {
                RemoveItem(menuItem);
                _items.Remove(menuItem);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var menuItem in _items)
                RemoveItem(menuItem);

            _items.Clear();
        }
    }

    private void RemoveItem(MenuItem menuItem)
    {
        menuItem.Checked -= MenuItem_Checked;
        menuItem.Unchecked -= MenuItem_Unchecked;
        menuItem.DataContext = null;

        ((IList) AssociatedObject.ItemsSource).Remove(menuItem);
    }

    private void AddItem(object itemVm, int index)
    {
        var menuItem = new MenuItem
        {
            Header = itemVm,
            DataContext = itemVm,
            IsCheckable = true,
            IsChecked = _comboboxFeature.CurrentItem == itemVm,
            IsEnabled = _comboboxFeature.IsEnabled,
            HeaderTemplate = _comboboxFeature.GetItemTemplate()
        };
        
        ((IList) AssociatedObject.ItemsSource).Insert(index, menuItem);

        menuItem.Checked += MenuItem_Checked;
        menuItem.Unchecked += MenuItem_Unchecked;

        _items.Add(menuItem);
    }
}