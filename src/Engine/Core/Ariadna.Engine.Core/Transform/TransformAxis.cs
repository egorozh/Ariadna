namespace Ariadna.Engine.Core
{
    public readonly struct TransformAxis
    {
        public bool IsTranslate { get; }

        public bool IsRotate { get; }

        public bool IsScale { get; }

        public TransformAxis(bool isTranslate = true, bool isRotate = false, bool isScale = false)
        {
            IsTranslate = isTranslate;
            IsRotate = isRotate;
            IsScale = isScale;
        }
    }
}