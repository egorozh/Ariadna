using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Fluent;

namespace Ariadna
{
    public class RibbonTabItem : Fluent.RibbonTabItem, IRibbonTabItem
    {
        #region Private Fields

        private readonly List<Control> _items = new List<Control>();

        #endregion

        #region Override Style

        static RibbonTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTabItem),
                new FrameworkPropertyMetadata(typeof(RibbonTabItem)));
        }

        #endregion

        #region Constructor

        public RibbonTabItem()
        {
            Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Public Methods

        public void AddRibbonItem(Control control, string? boxName)
        {
            _items.Add(control);

            var feature = control.GetBehavior<InterfaceFeatureBehavior>()?.Feature;

            if (feature != null)
            {
                feature.IsShowChanged += CommandFeature_IsShowChanged;

                if (feature.IsShow)
                    Visibility = Visibility.Visible;
            }
            
            var box = GetBox(boxName);

            if (box == null)
            {
                var newBox = new RibbonGroupBox
                {
                    Header = boxName,
                    ItemsSource = new ObservableCollection<UIElement>()
                    {
                        control
                    }
                };
                Groups.Add(newBox);
            }
            else
            {
                ((ObservableCollection<UIElement>) box.ItemsSource).Add(control);
            }
        }

        public void Block()
        {
            this.IsEnabled = false;

            foreach (var button in _items)
                button.IsEnabled = false;
        }

        public void UnBlock()
        {
            this.IsEnabled = true;

            foreach (var button in _items)
                button.IsEnabled = true;
        }

        public bool Contains(IFeature akimFeature)
        {
            foreach (var button in _items)
            {
                var commandFeature = button.GetBehavior<InterfaceFeatureBehavior>()?.Feature;

                if (commandFeature == akimFeature)
                    return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private RibbonGroupBox? GetBox(string? boxName)
        {
            foreach (var groupBox in Groups)
            {
                if (groupBox.Header as string == boxName)
                    return groupBox;
            }

            return null;
        }

        private void CommandFeature_IsShowChanged(object? sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;

            foreach (var control in _items)
            {
                var commandFeature = control.GetBehavior<InterfaceFeatureBehavior>().Feature;

                if (commandFeature.IsShow)
                {
                    Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        #endregion
    }
}