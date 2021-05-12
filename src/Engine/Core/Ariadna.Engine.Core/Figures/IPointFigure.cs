using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    public interface IPointFigure : ISelectedFigure2D
    {
        #region Properties

        double AnimateSpeed { get; set; }

        #endregion

        #region Events

        event EventHandler AnimationEnded;

        #endregion

        #region Methods

        PathGeometry GetGeometry();

        void Animate(IReadOnlyList<(double, Point)> animationSpline, double animateSpeed, bool showTimer = false);
        void StopAnimate();
        void ContinueAnimate(double animateSpeed);
        void PauseAnimate();

        #endregion
    }
}