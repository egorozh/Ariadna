using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ariadna.QuickActionsBar;

public class QuickActionsBar : WrapPanel
{
    private ObservableCollection<ObservableCollection<Control>> _prevGroups;

    #region Dependency Properties

    public static readonly DependencyProperty GroupsProperty = DependencyProperty.Register(
        nameof(Groups), typeof(ObservableCollection<ObservableCollection<Control>>), typeof(QuickActionsBar),
        new PropertyMetadata(default(ObservableCollection<ObservableCollection<Control>>), GroupsChanged));

    private static void GroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is QuickActionsBar actionsBar &&
            e.NewValue is ObservableCollection<ObservableCollection<Control>> groups)
        {
            actionsBar.GroupsChanged(groups);
        }
    }

    #endregion

    public QuickActionsBar()
    {
        Background = Brushes.Transparent;
    }

    #region Public Properties

    public ObservableCollection<ObservableCollection<Control>> Groups
    {
        get => (ObservableCollection<ObservableCollection<Control>>) GetValue(GroupsProperty);
        set => SetValue(GroupsProperty, value);
    }

    #endregion

    #region Private Methods

    private void GroupsChanged(ObservableCollection<ObservableCollection<Control>> groups)
    {
        if (_prevGroups != null)
            _prevGroups.CollectionChanged -= PrevButtons_CollectionChanged;

        if (groups == null)
        {
            _prevGroups = null;
            Children.Clear();
        }
        else
        {
            _prevGroups = groups;

            foreach (var group in groups)
            {
                var toolBarGroup = new ToolBarGroup
                {
                    Buttons = @group
                };
                toolBarGroup.ChangeOrientation(Orientation);

                Children.Add(toolBarGroup);
            }

            _prevGroups.CollectionChanged += PrevButtons_CollectionChanged;
        }
    }

    private void PrevButtons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var addedGroup = (ObservableCollection<Control>) e.NewItems[0];
            var toolBarGroup = new ToolBarGroup
            {
                Buttons = addedGroup
            };

            toolBarGroup.ChangeOrientation(Orientation);

            Children.Add(toolBarGroup);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            var removed = (ObservableCollection<Control>) e.OldItems[0];

            UIElement removedToolBarGroup = null;

            foreach (UIElement child in Children)
            {
                if (child is ToolBarGroup toolBar)
                {
                    if (toolBar.Buttons == removed)
                        removedToolBarGroup = toolBar;
                }
            }

            if (removedToolBarGroup != null)
                Children.Remove(removedToolBarGroup);
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            Children.Clear();
        }
    }

    #endregion
}