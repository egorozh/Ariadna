using Ariadna.Core;

namespace Ariadna
{
    public interface IMagicOptions
    {
        ThemeOptions Theme { get; }

        bool IsShowRibbon { get; set; }
    }

    public class ThemeOptions : BaseViewModel
    {
        public string Theme { get; set; }

        public string Accent { get; set; }  
    }
}