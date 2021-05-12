namespace Ariadna.Engine.Core
{
    /// <summary>
    /// ZIndex для примитивов движка
    /// </summary>
    public static class ZOrder
    {
        public static int MinFigureOrder { get; set; } = 0;

        public static int MaxFigureOrder { get; set; } = 1000;

        public static int OrtLine => MinFigureOrder - 1;
        public static int ImageFigure => MinFigureOrder - 2;


        public static int SelectedRectangle => MaxFigureOrder + 3;
        public static int SelectedFigure => MaxFigureOrder + 2;
        public static int CreationFigure => MaxFigureOrder + 1;
        public static int EditRectangle => MaxFigureOrder + 4;
        public static int EditRectanglePoint => MaxFigureOrder + 5;
        public static int EditPreviewFigure => MaxFigureOrder + 6;
        public static int EditSegment => MaxFigureOrder + 5;
        public static int EditNodePoint => MaxFigureOrder + 6;
        public static int StartEditNodePoint => MaxFigureOrder + 7;
        public static int HelpLine => MaxFigureOrder;

        public static int RulerTextBox => MaxFigureOrder + 10;
    }
}