using System;
using System.Collections.Generic;
using System.Linq;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.Transformation
{
    internal class TransformManager : ITransformManager
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;
        private ITransformManipulator? _manipulator;

        #endregion

        #region Events

        public event EventHandler<TransformActionEventArgs>? TransformEnded;

        #endregion

        #region Constructor

        public TransformManager(IAriadnaEngine ariadnaEngine)
        {
            _ariadnaEngine = ariadnaEngine;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            _ariadnaEngine.Figures.FigureSelected += (_, _) => UpdateManipulator();

            _ariadnaEngine.Figures.IsShowChanged += (_, _) => UpdateManipulator();
            _ariadnaEngine.Figures.IsFrozenChanged += (_, _) => UpdateManipulator();
            _ariadnaEngine.Figures.CollectionChanged += (_, _) => UpdateManipulator();

            _ariadnaEngine.Figures.DataChanged += (_, _) => Update();
            _ariadnaEngine.Figures.TransformChanged += (_, _) => Update();

            _ariadnaEngine.CoordinateSystem.CoordinateChanged += (_, e) =>
            {
                if (e.OldAngle != e.NewAngle)
                {
                    UpdateManipulator();
                }
            };
        }

        public void UpdateManipulator()
        {
            RemoveManipualtor();

            var selectedFigures = _ariadnaEngine.Figures.SelectedFigures
                .Where(figure => figure.IsShow && !figure.IsFrozen).ToList();

            switch (selectedFigures.Count)
            {
                case 0:

                    break;

                case 1:
                    CreateSingleManipulator(selectedFigures[0]);
                    break;

                default:
                    CreateGroupManipulator(selectedFigures);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void Update() => _manipulator?.Update();
        
        private void RemoveManipualtor()
        {
            if (_manipulator != null)
            {
                _manipulator.TransformEnded -= Manipulator_TransformEnded;

                _manipulator.Dispose();
            }

            _manipulator = null;
        }

        private void CreateSingleManipulator(ISelectedFigure2D selectedFigure)
        {
            var manipulator = new SingleManipulator(_ariadnaEngine, selectedFigure);
            manipulator.TransformEnded += Manipulator_TransformEnded;
            _manipulator = manipulator;
        }

        private void CreateGroupManipulator(List<ISelectedFigure2D> selectedFigures)
        {
            var manipulator = new GroupManipulator(_ariadnaEngine, selectedFigures);
            manipulator.TransformEnded += Manipulator_TransformEnded;
            _manipulator = manipulator;
        }

        private void Manipulator_TransformEnded(object? sender, TransformActionEventArgs e)
        {
            TransformEnded?.Invoke(this, e);
        }

        #endregion
    }
}