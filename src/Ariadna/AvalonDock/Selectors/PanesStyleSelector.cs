using System.Windows;
using System.Windows.Controls;

namespace Ariadna
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle { get; set; }

        public Style DocumentStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            return item switch
            {
                IDocumentViewModel _ => DocumentStyle,
                IToolViewModel _ => ToolStyle,
                _ => base.SelectStyle(item, container)
            };
        }
    }   
}