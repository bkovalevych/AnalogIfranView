using System.Windows.Input;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.ViewModels
{
    using Helpers;

    public class PageHolder : Observable
    {
        public UndoRedoViewModel UndoRedoViewModel { get => undoRedoViewModel; set => Set(ref undoRedoViewModel, value); }
        private UndoRedoViewModel undoRedoViewModel;

        public Images Images { get => images; set => Set(ref images, value); }
        private Images images;
        private InkPresenter presenter;
        public ICommand InitPresenterCommand => new RelayCommand(InitPresenter);
        private void InitPresenter(object o) {
            if (o is InkPresenter pr) {
                presenter = pr;
                images.Presenter = presenter;
                undoRedoViewModel.Presenter = presenter;
                presenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse 
                    | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            }
        }
        
        public PageHolder() {
            images = new Images();
            undoRedoViewModel = new UndoRedoViewModel();
        }
    }
}
