using Windows.UI.Input.Inking;
using System.Windows.Input;

namespace AnalogIfranView.ViewModels
{
    using Models;
    using Helpers;
    
    public class UndoRedoViewModel : Observable
    {
        private readonly UndoRedoService service;
        public UndoRedoViewModel() {
            service = new UndoRedoService();
        }
        public InkPresenter Presenter { get => presenter; set
            {
                service.ObservePresenter(value);
                Set(ref presenter, value);
            }
        }
        private InkPresenter presenter;
        public ICommand UndoCommand => new RelayCommand(Undo);
        private void Undo(object o) {
            service.UndoOperation();
        }

        public ICommand RedoCommand => new RelayCommand(Redo);
        private void Redo(object o) {
            service.RedoOperation();
        }
    }
}
