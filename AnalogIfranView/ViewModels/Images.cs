using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Windows.Input;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using IOServices;
    using System.Diagnostics;
    using Windows.Graphics.Imaging;
    using Windows.UI.Input.Inking;
    using Windows.UI.Xaml.Controls;

    public class Images : Observable
    {
        public double Zoom
        {
            get => zoom;
            set {
                Set(ref zoom, value);
            }
        }
        private double zoom = 1.0;
        public BitmapImage Image
        {
            get => image;
            set => Set(ref image, value);
        }
        private BitmapImage image;
        public double Width
        {
            get => width;
            set => Set(ref width, value);
        }
        private double width = 100;

        public double Height
        {
            get => height;
            set => Set(ref height, value);
        }
        private double height = 100;
 
        public ICommand OpenImageCommand => new RelayCommand(OpenFileFunction);
        private async void OpenFileFunction(object param) {
            BitmapImage openedValue = await imgOpener.OpenImageDialog();
            if (openedValue == null) {
                return;
            }
            Image = openedValue;
            
            Width = openedValue.DecodePixelWidth;
            Height = openedValue.DecodePixelHeight;
        }


        public ICommand SaveImageCommand => new RelayCommand(SaveFileFunction);
        private async void SaveFileFunction(object param) {
            await imgOpener.SaveImageDialog();
        }

        public ICommand DrawCanvasCommand => new RelayCommand(DrawCanvas);
        private void DrawCanvas(object param) {
            var canvas = param as InkCanvas;

            Trace.WriteLine("hello");
        }
        private readonly ImageDialogOpener imgOpener;
        public Images(InkStrokeContainer container) {
            imgOpener = new ImageDialogOpener(container);
        }


        public InkStrokeContainer Strokes
        {
            get => strokes;
            set => Set(ref strokes, value);
        }
        private InkStrokeContainer strokes;
    }
}
