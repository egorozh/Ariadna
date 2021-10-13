using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Ariadna.QuickActionsBar;

public partial class AkimToolBarGroup
{
    #region Dependency Properties

    public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(
        nameof(Buttons), typeof(ObservableCollection<Control>), typeof(AkimToolBarGroup),
        new PropertyMetadata(default(ObservableCollection<Control>), ButtonsChanged));

    private ObservableCollection<Control> _prevButtons;

    private static void ButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AkimToolBarGroup akimToolBarGroup && e.NewValue is ObservableCollection<Control> buttons)
        {
            akimToolBarGroup.ButtonsChanged(buttons);
        }
    }

    #endregion

    #region Public Properties

    public ObservableCollection<Control> Buttons
    {
        get => (ObservableCollection<Control>) GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    public void ChangeOrientation(Orientation orientation)
    {
        Container.Orientation = orientation;
    }

    #endregion

    #region Constructor

    public AkimToolBarGroup()
    {
        InitializeComponent();
    }

    #endregion

    #region Private Methods

    private void ButtonsChanged(ObservableCollection<Control> buttons)
    {
        if (_prevButtons != null)
            _prevButtons.CollectionChanged -= PrevButtons_CollectionChanged;

        if (buttons == null)
        {
            _prevButtons = null;
            Container.Children.Clear();
        }
        else
        {
            _prevButtons = buttons;

            foreach (var buttonBase in buttons)
            {
                Container.Children.Add(buttonBase);
            }

            _prevButtons.CollectionChanged += PrevButtons_CollectionChanged;
        }
    }

    private void PrevButtons_CollectionChanged(object sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var addedButton = (Control) e.NewItems[0];
            Container.Children.Add(addedButton);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            var removedButton = (Control) e.OldItems[0];
            Container.Children.Remove(removedButton);
        }
    }

    private void OrientationChanged(Orientation orientation)
    {
        Container.Orientation = orientation;
    }

    #endregion
}