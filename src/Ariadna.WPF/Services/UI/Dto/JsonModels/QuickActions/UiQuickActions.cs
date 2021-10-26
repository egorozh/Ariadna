using System.Collections.Generic;

namespace Ariadna;

public class UiQuickActions
{
    public bool IsShowLeft { get; set; }
    public bool IsShowTop { get; set; }
    public bool IsShowRight { get; set; }

    public RibbonItemSize Size { get; set; }

    public List<UiQuickActionsGroup> Left { get; set; } = new();
    public List<UiQuickActionsGroup> Top { get; set; } = new();
    public List<UiQuickActionsGroup> Right { get; set; } = new();
}