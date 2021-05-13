using System.Windows.Input;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using Services;

    public class UndoRedoViewModel : Observable
    {
        private readonly UndoRedoService service;
        public UndoRedoViewModel()
        {
            service = new UndoRedoService();
        }
        public InkPresenter Presenter
        {
            get => presenter; set
            {
                service.ObservePresenter(value);
                Set(ref presenter, value);
            }
        }
        private InkPresenter presenter;
        public ICommand UndoCommand => new RelayCommand(Undo);
        private void Undo(object o)
        {
            service.UndoOperation();
        }

        public ICommand RedoCommand => new RelayCommand(Redo);
        private void Redo(object o)
        {
            service.RedoOperation();
        }
    }
}
