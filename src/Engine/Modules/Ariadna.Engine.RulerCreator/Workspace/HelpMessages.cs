namespace AKIM.Engine.TwoD.RulerCreator
{
    internal static class HelpMessages
    {
        public static string SetMiddleArcMessage { get; } = "Укажите промежуточную точку, лежащую на дуге";

        public static string SetStartPointMessage { get; } = "Укажите стартовую точку на холсте";

        public static string SetNextPointMessage { get; } = "Укажите следующую точку геометрии";

        public static string CreateWarningTwoSegmentsMessage { get; } =
            "Нельзя создать фигуру менее, чем из 2 сегментов!";

        public static string CreateWarningOneSegmentMessage { get; } = "Нельзя создать фигуру без одного сегмента!";

        public static string SelectNodeOrSegment { get; } = "Выберите сегмент или узловую точку!";

        public static string CreateWarningWithoutStartPointMessage { get; } =
            "Нельзя создать фигуру, не указав стартовую точку";

        public static string SetEndPointArcMessage { get; } = "Укажите конечную точку дуги";

        public static string SelectEdgePointMessage { get; } =
            "Выделите одну из конечных точек (красного цвета), \r\nс которых будет продолжена геометрия";
    }
}