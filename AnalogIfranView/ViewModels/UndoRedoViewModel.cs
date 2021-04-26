using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalogIfranView.ViewModels
{
    using Models;
    using Windows.UI.Input.Inking;
    using Helpers;
    using System.Windows.Input;

    public class UndoRedoViewModel
    {
        private UndoRedoService service;
        public UndoRedoViewModel(InkPresenter inkPresenter) {
            service = new UndoRedoService();
            service.ObservePresenter(inkPresenter);
        }

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
