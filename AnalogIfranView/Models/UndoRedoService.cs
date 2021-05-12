using AnalogIfranView.Helpers;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.Models
{
    public class UndoRedoService
    {
        private readonly Stack<BaseOperation> stackOfUndo = new Stack<BaseOperation>();
        private readonly Stack<BaseOperation> currentOperations = new Stack<BaseOperation>();
        private InkPresenter presenter;
        
        public void RedoOperation()
        {
            if(stackOfUndo.Count == 0)
            {
                return;
            }
            currentOperations.Push(stackOfUndo.Pop());
            var operation = currentOperations.Peek();
            RestoreValue(operation);
        }

        public void UndoOperation()
        {
            if(currentOperations.Count == 0)
            {
                return;
            }
            stackOfUndo.Push(currentOperations.Pop());
            var operation = stackOfUndo.Peek();
            ClearValue(operation);
        }

        private void ClearValue(BaseOperation operation)
        {
            if(operation is Operation<InkStroke> currentOperation)
            {
                InkStroke s = currentOperation.Instance;
                s.Selected = true;
                presenter.StrokeContainer.DeleteSelected();
            }
        }

        private void RestoreValue(BaseOperation operation)
        {
            if(operation is Operation<InkStroke> currentOperation)
            {
                InkStroke stroke = currentOperation.Instance;
                InkStrokeBuilder strokeBuilder = new InkStrokeBuilder();
                strokeBuilder.SetDefaultDrawingAttributes(stroke.DrawingAttributes);
                System.Numerics.Matrix3x2 matr = stroke.PointTransform;
                IReadOnlyList<InkPoint> inkPoints = stroke.GetInkPoints();
                InkStroke stk = strokeBuilder.CreateStrokeFromInkPoints(inkPoints, matr);

                presenter.StrokeContainer.AddStroke(stk);
                currentOperation.Instance = stk;
            }
        }

        public void ObservePresenter(InkPresenter presenter)
        {
            this.presenter = presenter;
            if(presenter != null)
            {
                presenter.StrokesCollected += Presenter_StrokesCollected;
            }
        }

        public ICommand ObservePresenterCommand => new RelayCommand((o) =>
        {
            if(o is InkPresenter pr)
            {
                ObservePresenter(pr);
            }
        });

        private void Presenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            foreach(var stroke in args.Strokes)
            {
                currentOperations.Push(new Operation<InkStroke>() { Instance = stroke });
            }
        }

        public class BaseOperation
        {

        }
        
        public class Operation<T> : BaseOperation
        {
            public T Instance;
        }
    }
}
