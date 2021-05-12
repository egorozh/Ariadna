using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Ariadna
{
    public interface IComboboxFeature : IInterfaceFeature
    {
        bool IsEnabled { get; set; }

        IViewModelItemCollection Items { get; }

        object SelectedItem { get; set; }
        DataTemplate ItemTemplate { get; set; }

        /// <summary>
        /// Название свойства во View-Model Item'а, по которому будет связь с menuItem'ом в <see cref="MenuItem.Header"/>
        /// </summary>
        string MenuItemHeaderNameProperty { get; }

        event EventHandler CurrentChangedEvent;
        event EventHandler SelectionChanged;
        event EventHandler IsEnabledChanged;
    }

    public interface IViewModelItemCollection : INotifyCollectionChanged, IEnumerable
    {
    }

    public sealed class ViewModelItemCollection<T> : ObservableCollection<T>, IViewModelItemCollection
    {
    }
}