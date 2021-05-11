using AnalogIfranView.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.Models
{
    public class UndoRedoService
    {
        public void RedoOperation() {
            if (stackOfUndo.Count == 0) {
                return;
            }
            currentOperations.Push(stackOfUndo.Pop());
            var operation = currentOperations.Peek();
            RestoreValue(operation);
        }

        public void UndoOperation() {
            if (currentOperations.Count == 0) {
                return;
            }
            stackOfUndo.Push(currentOperations.Pop());
            var operation = stackOfUndo.Peek();
            ClearValue(operation);
            
        }
        private void ClearValue(BaseOperation operation) {
            if (operation.GetType() == typeof(Operation<InkStroke>)) {
                InkStroke s = ((Operation<InkStroke>)operation).Instance;
                s.Selected = true;
                presenter.StrokeContainer.DeleteSelected();
            }
        }

        private void RestoreValue(BaseOperation operation) {
            if (operation.GetType() == typeof(Operation<InkStroke>)) {
                Operation<InkStroke> currentOperation = (Operation<InkStroke>)operation;
                InkStroke stroke = ((Operation<InkStroke>)operation).Instance;
                InkStrokeBuilder strokeBuilder = new InkStrokeBuilder();
                strokeBuilder.SetDefaultDrawingAttributes(stroke.DrawingAttributes);
                System.Numerics.Matrix3x2 matr = stroke.PointTransform;
                IReadOnlyList<InkPoint> inkPoints = stroke.GetInkPoints();
                InkStroke stk = strokeBuilder.CreateStrokeFromInkPoints(inkPoints, matr);
                
                presenter.StrokeContainer.AddStroke(stk);
                currentOperation.Instance = stk;
            }
        }

        public void ObservePresenter(InkPresenter presenter) {
            this.presenter = presenter;
            if (presenter != null) {
                presenter.StrokesCollected += Presenter_StrokesCollected;
            }
        }

        public ICommand ObservePresenterCommand => new RelayCommand((o) =>
        {
            if (o is InkPresenter pr) {
                ObservePresenter(pr);
            }
        });

        private void Presenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args) {
            foreach (var stroke in args.Strokes) {
                currentOperations.Push(new Operation<InkStroke>() { Instance = stroke });
            }
        }

        private Stack<BaseOperation> stackOfUndo = new Stack<BaseOperation>();
        private Stack<BaseOperation> currentOperations = new Stack<BaseOperation>();
        private InkPresenter presenter;
        public class BaseOperation
        {
            
        }
        public class Operation <T> : BaseOperation 
        {
            public T Instance;
        }

    }
}
