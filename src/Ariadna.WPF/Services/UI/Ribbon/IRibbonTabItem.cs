using System.Windows.Controls;

namespace Ariadna;

internal interface IRibbonTabItem
{
    object Header { get; set; }

    bool IsEnabled { get; set; }

    void AddRibbonItem(Control control, string boxName);
    void Block();
    void UnBlock();
    bool Contains(IFeature feature);
}