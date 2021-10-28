using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ariadna;

public abstract class ComboboxFeature<T> : InterfaceFeature, IComboboxFeature<T>
    where T : class, IComboBoxItem
{
    #region Private Fields

    private bool _isEnabled;
    private T? _currentItem;

    #endregion

    #region Public Properties

    public ObservableCollection<T> Items { get; } = new();

    object? IComboboxFeature.CurrentItem
    {
        get => CurrentItem;
        set => CurrentItem = value as T;
    }

    System.Collections.IEnumerable IComboboxFeature.Items => Items;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            IsEnabledChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public T? CurrentItem
    {
        get => _currentItem;
        set
        {
            _currentItem = value;
            CurrentChanged?.Invoke(this, EventArgs.Empty);
            OnCurrentChanged();
        }
    }

    #endregion

    #region Events

    public event EventHandler? CurrentChanged;
    public event EventHandler? IsEnabledChanged;

    #endregion

    #region Public Methods

    public virtual DataTemplate GetItemTemplate()
    {
        var factory = new FrameworkElementFactory(typeof(TextBlock));

        factory.SetBinding(TextBlock.TextProperty,
            new Binding(nameof(IComboBoxItem.Header)));

        factory.SetValue(TextBlock.FontSizeProperty, 12.0);

        return new DataTemplate(typeof(T))
        {
            VisualTree = factory
        };
    }

    #endregion

    protected virtual void OnCurrentChanged()
    {
    }
}