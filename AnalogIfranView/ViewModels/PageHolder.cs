using System.Windows.Input;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.ViewModels
{
    using Helpers;

    public class PageHolder : Observable
    {
        public UndoRedoViewModel UndoRedoViewModel
        {
            get => undoRedoViewModel; set => Set(ref undoRedoViewModel, value);
        }
        
        private UndoRedoViewModel undoRedoViewModel;

        public ThumbnailViewModel ThumbnailViewModel
        {
            get => images; set => Set(ref images, value);
        }
        
        private ThumbnailViewModel images;
        public InkPresenter Presenter => presenter;
        private InkPresenter presenter;
        
        public ICommand InitPresenterCommand => new RelayCommand(InitPresenter);
        
        private void InitPresenter(object o)
        {
            if(o is InkPresenter pr)
            {
                pr.StrokeContainer.Clear();
                presenter = pr;
                images.Presenter = presenter;
                undoRedoViewModel.Presenter = presenter;
                presenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                    | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            }
        }

        public PageHolder()
        {
            images = new ThumbnailViewModel();
            undoRedoViewModel = new UndoRedoViewModel();
        }
    }
}
