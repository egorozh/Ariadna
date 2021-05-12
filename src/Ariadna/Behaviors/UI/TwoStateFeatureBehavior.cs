using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Fluent;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
    public class TwoStateFeatureBehavior : Behavior<Control>
    {
        private string header;
        private string headerAlt;
        private object icon;
        private object iconAlt;
        private List<UiHelpVideo> _videos;
        private List<UiKeyBinding> _hotKeys;

        private KeyBinding kb;
        private UiQuickActionItem _item;

        public static readonly DependencyProperty TwoStateCommandFeatureProperty = DependencyProperty.Register(
            nameof(TwoStateCommandFeature), typeof(ITwoStateCommandFeature), typeof(TwoStateFeatureBehavior),
            new PropertyMetadata(default(ITwoStateCommandFeature)));


        public ITwoStateCommandFeature TwoStateCommandFeature
        {
            get => (ITwoStateCommandFeature) GetValue(TwoStateCommandFeatureProperty);
            set => SetValue(TwoStateCommandFeatureProperty, value);
        }

        public TwoStateFeatureBehavior(ITwoStateCommandFeature feature)
        {
            TwoStateCommandFeature = feature;
        }

        public TwoStateFeatureBehavior(ITwoStateCommandFeature feature, string header, string headerAlt, object icon,
            object iconAlt, KeyBinding kb, UiQuickActionItem item) : this(feature)
        {
            this.kb = kb;
            _item = item;
            this.header = header;
            this.headerAlt = headerAlt;
            this.icon = icon;
            this.iconAlt = iconAlt;
        }

        protected override void OnAttached()
        {
            TwoStateCommandFeature.StateChanged += TwoStateCommandFeature_StateChanged;
        }

        protected override void OnDetaching()
        {
            TwoStateCommandFeature.StateChanged -= TwoStateCommandFeature_StateChanged;
        }

        private void TwoStateCommandFeature_StateChanged(object sender, System.EventArgs e)
        {
            if (TwoStateCommandFeature.State == State.Main)
            {
                if (AssociatedObject is ButtonBase button)
                {
                    button.ToolTip =
                        TwoStateCommandFeature.AriadnaApp.InterfaceHelper.CreateToolTip(TwoStateCommandFeature,
                            _videos, _hotKeys, _item.Header, _item.Description, _item.DisableReason);
                }

                if (AssociatedObject is IRibbonControl ribbonControl)
                {
                    ribbonControl.Header = header;
                    ribbonControl.Icon = icon;
                }

                if (AssociatedObject is ILargeIconProvider largeIconProvider)
                    largeIconProvider.LargeIcon = icon;
            }
            else
            {
                if (AssociatedObject is ButtonBase button)
                {
                    button.ToolTip =
                        TwoStateCommandFeature.AriadnaApp.InterfaceHelper.CreateToolTip(TwoStateCommandFeature, _videos,
                            _hotKeys, _item.AlternativeHeader, _item.AlternativeDescription,
                            _item.AlternativeDisableReason);
                }

                if (AssociatedObject is IRibbonControl ribbonControl)
                {
                    ribbonControl.Header = headerAlt;
                    ribbonControl.Icon = iconAlt;
                }

                if (AssociatedObject is ILargeIconProvider largeIconProvider)
                    largeIconProvider.LargeIcon = iconAlt;
            }
        }
    }
}