using System.Collections.Generic;

namespace Ariadna
{
    public class UiMenuItem
    {
        public string Header { get; set; }

        public List<UiMenuItem> Children { get; set; } = new List<UiMenuItem>();

        public string Id { get; set; }
    }
}