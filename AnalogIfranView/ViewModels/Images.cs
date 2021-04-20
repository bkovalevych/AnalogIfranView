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

    public class Images : Observable
    {
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
            Image = await imgOpener.OpenImageDialog();
            Width = Image.DecodePixelWidth;
            Height = Image.DecodePixelHeight;
        }
        private ImageOpener imgOpener;
        public Images() {
            imgOpener = new ImageOpener();
        }
    }
}
