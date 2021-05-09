using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using Models;
    using IOServices;
    using System.Windows.Input;
    using Windows.UI.Input.Inking;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

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
                undoRedoViewModel = new UndoRedoViewModel(presenter);
                presenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse 
                    | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            }
            if (o is ContentPresenter cp) {
                //Almost Done int childrenCount = VisualTreeHelper.GetChildrenCount();
            }
        }
        public PageHolder() {
            images = new Images();

        }

    }
}
