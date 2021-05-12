using System.Collections.Generic;

namespace Ariadna
{
    public class UiRibbonGroup
    {
        public string Header { get; set; }

        public List<UiRibbonItem> Items { get; set; } = new();
    }
}