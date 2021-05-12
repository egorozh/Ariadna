using System.Windows;
using System.Windows.Controls;

namespace Ariadna
{
    public class PanesTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                IDocumentViewModel documentViewModel => documentViewModel.GetTemplate(),
                IToolViewModel toolViewModel => toolViewModel.GetTemplate(),
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}