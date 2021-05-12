using System.Collections.Generic;

namespace Ariadna
{
    public class UiSplitButtonItem
    {
        public string Header { get; set; }

        public List<UiRibbonItem> Items { get; set; } = new();
    }
}