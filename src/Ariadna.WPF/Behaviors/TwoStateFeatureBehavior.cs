using Fluent;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Ariadna;

public class TwoStateFeatureBehavior : Behavior<Control>
{
    private readonly string _header;
    private readonly string _headerAlt;
    private readonly object? _icon;
    private readonly object? _iconAlt;
    private readonly List<UiHelpVideo>? _videos = null;
    private readonly List<UiKeyBinding>? _hotKeys = null;

    private readonly UiQuickActionItem _item;
    private ITwoStateCommandFeature _twoStateCommandFeature;
    private IInterfaceHelper _interfaceHelper;

    public TwoStateFeatureBehavior(ITwoStateCommandFeature feature, IInterfaceHelper interfaceHelper, string header,
        string headerAlt, object icon,
        object iconAlt, UiQuickActionItem item)
    {
        _twoStateCommandFeature = feature;
        _interfaceHelper = interfaceHelper;

        _item = item;
        _header = header;
        _headerAlt = headerAlt;
        _icon = icon;
        _iconAlt = iconAlt;
    }

    protected override void OnAttached()
    {
        _twoStateCommandFeature.StateChanged += TwoStateCommandFeature_StateChanged;
    }

    protected override void OnDetaching()
    {
        _twoStateCommandFeature.StateChanged -= TwoStateCommandFeature_StateChanged;
        _twoStateCommandFeature = null!;
        _interfaceHelper = null!;
    }

    private void TwoStateCommandFeature_StateChanged(object? sender, EventArgs e)
    {
        if (_twoStateCommandFeature.State == State.Main)
        {
            if (AssociatedObject is ButtonBase button)
            {
                button.ToolTip =
                    _interfaceHelper.CreateToolTip(_twoStateCommandFeature,
                        _videos, _hotKeys, _item.Header, _item.Description, _item.DisableReason);

                button.Content = _icon;
            }

            if (AssociatedObject is IRibbonControl ribbonControl)
            {
                ribbonControl.Header = _header;
                ribbonControl.Icon = _icon;
            }

            if (AssociatedObject is ILargeIconProvider largeIconProvider)
                largeIconProvider.LargeIcon = _icon;
        }
        else
        {
            if (AssociatedObject is ButtonBase button)
            {
                button.ToolTip =
                    _interfaceHelper.CreateToolTip(_twoStateCommandFeature, _videos,
                        _hotKeys, _headerAlt, _item.AlternativeDescription,
                        _item.AlternativeDisableReason);

                button.Content = _iconAlt;
            }

            if (AssociatedObject is IRibbonControl ribbonControl)
            {
                ribbonControl.Header = _headerAlt;
                ribbonControl.Icon = _iconAlt;
            }

            if (AssociatedObject is ILargeIconProvider largeIconProvider)
                largeIconProvider.LargeIcon = _iconAlt;
        }
    }
}