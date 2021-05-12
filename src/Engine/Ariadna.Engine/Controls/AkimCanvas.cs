using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    internal class AkimCanvas : Canvas, ICanvas
    {
        #region Constructor

        public AkimCanvas()
        {
            Background = Brushes.Transparent;
            ClipToBounds = true;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
        }

        public void AddElements(params UIElement[] elements)
        {
            foreach (var uiElement in elements)
                Children.Add(uiElement);
        }

        public void RemoveElements(params UIElement?[] elements)
        {
            foreach (var element in elements)
            {
                if (element != null)
                    Children.Remove(element);
            }
        }

        #endregion
    }
}