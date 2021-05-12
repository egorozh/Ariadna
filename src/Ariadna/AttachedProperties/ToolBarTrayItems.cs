using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Ariadna.Core;

namespace Ariadna
{
    /// <summary>
    /// Привязка коллекции <see cref="ToolBar"/> к <see cref="ToolBarTray"/>
    /// </summary>
    public class ToolBarTrayItems : BaseAttachedProperty<ToolBarTrayItems, ObservableCollection<ToolBar>>
    {
        private ToolBarTray _toolBarTray;

        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ToolBarTray toolBarTray)) return;

            _toolBarTray = toolBarTray;

            if (!(e.NewValue is ObservableCollection<ToolBar> toolBars)) return;

            toolBars.CollectionChanged += ToolBarsCollectionChanged;


            foreach (var toolBar in toolBars)
                toolBarTray.ToolBars.Add(toolBar);
        }

        private void ToolBarsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                _toolBarTray.ToolBars.Add((ToolBar) e.NewItems[0]);

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                _toolBarTray.ToolBars.Remove((ToolBar) e.OldItems[0]);

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _toolBarTray.ToolBars.Clear();

                return;
            }
        }
    }
}