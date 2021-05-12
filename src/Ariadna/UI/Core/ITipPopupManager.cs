using System;

namespace Ariadna
{
    public interface ITipPopupManager
    {
        bool IsOpen { get; set; }

        TipPopupViewModel Context { get; set; }
    }

    public class TipPopupViewModel
    {
        public string Header { get; set; }

        public string Description { get; set; }
        
        public string DisableReason { get; set; }

        public Uri HelpVideo { get; set; }
    }
}