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
                var children = VisualTreeHelper.GetChild(cp, 0);
                var parent = VisualTreeHelper.GetParent(cp);
                StackPanel sp = VisualTreeHelper.GetChild(children, 0) as StackPanel;
                InkToolbar toolbar = sp.Children[0] as InkToolbar;

                ScrollViewer sw = VisualTreeHelper.GetChild(children, 1) as ScrollViewer;
                var content = VisualTreeHelper.GetChild(sw, 0);
                var border = (VisualTreeHelper.GetChild((VisualTreeHelper.GetChild(content, 0) as Grid).Children[0], 0) as Grid).Children[1] as Border;
                InkCanvas canvas = border.Child as InkCanvas;
                // if no border
                toolbar.TargetInkCanvas = canvas;
                presenter = canvas.InkPresenter;
                images.Presenter = presenter;
                undoRedoViewModel = new UndoRedoViewModel(presenter);
                presenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                    | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            }
        }
        
        public PageHolder() {
            images = new Images();

        }

    }
}
