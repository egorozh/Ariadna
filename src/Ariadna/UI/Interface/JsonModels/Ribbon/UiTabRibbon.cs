using System.Collections.Generic;

namespace Ariadna
{
    public class UiTabRibbon
    {
        public string Header { get; set; }

        public List<UiRibbonGroup> Boxes { get; set; } = new List<UiRibbonGroup>();
    }
}