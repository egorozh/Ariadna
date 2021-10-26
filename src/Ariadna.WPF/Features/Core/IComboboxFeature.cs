using System.Collections.ObjectModel;
using System.Windows;

namespace Ariadna;

public interface IComboboxFeature<T> : IComboboxFeature where T : IComboBoxItem
{
    new ObservableCollection<T> Items { get; }
    new T? CurrentItem { get; set; }
}

public interface IComboboxFeature : IInterfaceFeature
{
    System.Collections.IEnumerable Items { get; }
    object? CurrentItem { get; set; }

    bool IsEnabled { get; set; }

    event EventHandler CurrentChanged;
    event EventHandler IsEnabledChanged;

    DataTemplate GetItemTemplate();
}

public interface IComboBoxItem
{
    string Header { get; }
}