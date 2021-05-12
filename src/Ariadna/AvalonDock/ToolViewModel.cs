using System.Windows.Media.Imaging;

namespace Ariadna
{
    /// <summary>
    /// Класс модели инструмента редактора
    /// </summary>
    public abstract class ToolViewModel : PaneViewModel, IToolViewModel
    {
        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; } = true;

        public virtual string Name { get; }

        protected ToolViewModel(string name)
        {
            Name = name;
            Title = name;
            IconSource = new BitmapImage();
        }
    }

    public interface IToolViewModel : IPaneViewModel
    {
        bool IsVisible { get; set; }
        bool IsEnabled { get; set; }

        string Name { get; }
    }
}