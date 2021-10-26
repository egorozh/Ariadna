using System.Collections.ObjectModel;
using System.Windows;

namespace Ariadna;

public interface IComboboxFeature<T> : IInterfaceFeature where T : IComboBoxItem
{
    bool IsEnabled { get; set; }

    ObservableCollection<T> Items { get; }
    T? CurrentItem { get; set; }
        
    event EventHandler CurrentChanged;
    event EventHandler IsEnabledChanged;

    DataTemplate GetItemTemplate();
}

public interface IComboBoxItem
{
    string Header { get; }
}