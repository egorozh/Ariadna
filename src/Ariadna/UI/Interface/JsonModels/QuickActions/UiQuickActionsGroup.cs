using System.Collections.Generic;

namespace Ariadna
{
    public class UiQuickActionsGroup
    {
        public string Header { get; set; }

        public List<UiQuickActionItem> Items { get; set; } = new List<UiQuickActionItem>();
    }
}