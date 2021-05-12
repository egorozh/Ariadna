using Ariadna.Core;

namespace Ariadna
{
    internal class TipPopupManager : BaseViewModel, ITipPopupManager
    {
        public bool IsOpen { get; set; }

        public TipPopupViewModel Context { get; set; }
    }
}