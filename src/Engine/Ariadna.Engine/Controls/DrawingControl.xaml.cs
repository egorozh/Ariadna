using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Основной контрол движка
    /// </summary>
    internal partial class DrawingControl : IDrawingControl
    {
        private readonly IAriadnaEngine _ariadnaEngine;

        #region Public Properties

        /// <summary>
        /// Панель помощи с текущими координатами указателя мыши, текущим разрешением,
        /// кнопками включения сетки, режимов примагничивания
        /// </summary>
        public HelpPanelViewModel HelpPanelViewModel { get; }

        /// <summary>
        /// Подложка
        /// </summary>
        public ContentControl SubstrateControl => SubstrateControlField;

        /// <summary>
        /// Подложка с сеткой
        /// </summary>
        public IGridCanvas GridCanvas { get; }

        public IImagesCanvas ImagesCanvas { get; }

        public INavigationGrid NavigationGrid { get; }

        /// <summary>
        /// Холст, отображающий все примитивы движка
        /// </summary>
        public ICanvas Canvas { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Дефолтный конструктор
        /// </summary>
        public DrawingControl(HelpPanelViewModel helpPanelViewModel, IGridCanvas gridCanvas, IAriadnaEngine ariadnaEngine,
            IImagesCanvas imagesCanvas, INavigationGrid navigationGrid)
        {
            _ariadnaEngine = ariadnaEngine;
            InitializeComponent();

            GridCanvas = gridCanvas;
            ImagesCanvas = imagesCanvas;
            NavigationGrid = navigationGrid;

            HelpPanelViewModel = helpPanelViewModel;
            HelpPanelControl.DataContext = helpPanelViewModel;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            Canvas = _ariadnaEngine.Canvas;

            CanvasControl.Content = Canvas;
            GridControlField.Content = GridCanvas;
            ImagesControlField.Content = ImagesCanvas;
            NavigationControlField.Content = NavigationGrid;

            Background = Brushes.Transparent;

            if (ColorConverter.ConvertFromString(EngineGlobalSettings.Instance.BackgroundColor) is Color color)
                Background = new SolidColorBrush(color);

            EngineGlobalSettings.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        /// <summary>
        /// Показ подсказки внизу слева
        /// </summary>
        /// <param name="message">Текст подсказки</param>
        /// <param name="isWarning"></param>
        public void ShowMessage(string message, bool isWarning = false)
        {
            if (string.IsNullOrEmpty(message))
            {
                MessageText.Text = "";
                Border.Visibility = Visibility.Collapsed;
                return;
            }

            Border.Visibility = Visibility.Visible;
            Border.Background = isWarning ? Brushes.Red : Brushes.NavajoWhite;

            if (isWarning)
            {
                var animation = new ThicknessAnimation(new Thickness(1, 5, 9, 5), new Thickness(9, 5, 1, 5),
                    new Duration(new TimeSpan(0, 0, 0, 0, 50)), FillBehavior.Stop);

                animation.AutoReverse = true;
                animation.RepeatBehavior = new RepeatBehavior(5);

                Border.BeginAnimation(MarginProperty, animation);
            }


            MessageText.Foreground = isWarning ? Brushes.White : Brushes.Black;
            MessageText.Text = message;
        }

        #endregion

        #region Private Methods

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EngineGlobalSettings.BackgroundColor))
            {
                if (ColorConverter.ConvertFromString(EngineGlobalSettings.Instance.BackgroundColor) is Color color)
                    Background = new SolidColorBrush(color);
            }
        }

        #endregion
    }
}