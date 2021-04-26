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
    using Models;
    using Windows.UI.Xaml;

    public class Images : Observable
    {
        public string NamePicture 
        { 
            get => namePicture;
            set => Set(ref namePicture, value); 
        }
        private string namePicture = "New picture";
        public double Zoom
        {
            get => zoom;
            set {
                zoom = value;
                OnPropertyChanged("");
            }
        }
        private double zoom = 1.0;

        
        
        public double ScaledWidth
        {
            get => width * zoom;
            set { }
        }
        

        public double ScaledHeight
        {
            get => zoom * height;
            set { }
        }
        

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
        private double width = 800;

        public double Height
        {
            get => height;
            set => Set(ref height, value);
        }
        private double height = 400;
 
        public ICommand OpenImageCommand => new RelayCommand(OpenFileFunction);
        private async void OpenFileFunction(object param) {
            holst = await imgOpener.OpenImageDialog();
            BitmapImage openedValue = ((ImageHolst)holst).imageSRC;
            if (openedValue == null) {
                return;
            }
            Image = openedValue;
            Width = openedValue.DecodePixelWidth;
            Height = openedValue.DecodePixelHeight;
        }


        public ICommand SaveImageCommand => new RelayCommand(SaveFileFunction);
        private async void SaveFileFunction(object param) {
            await imgOpener.SaveImageDialog(holst);
        }

        
        public async Task<bool> Save() {
            bool result = await imgOpener.SaveOnClose(holst);
            return result;
        }
        public ICommand SaveCommand => new RelayCommand(async (o) => await imgOpener.Save(holst));
        private readonly ImageDialogOpener imgOpener;
        public Images(InkStrokeContainer container) {
            imgOpener = new ImageDialogOpener(container);
            holst = new ThumbnailHolst() { Height = (int)Height, Width = (int)Width, Name = namePicture };
        }

        
        public InkStrokeContainer Strokes
        {
            get => strokes;
            set => Set(ref strokes, value);
        }

        public void InitByHolst(IHolst holst) {
            this.holst = holst;
            Width = holst.Width;
            Height = holst.Height;
            NamePicture = holst.Name;
        }

        private InkStrokeContainer strokes;
        private IHolst holst;

    }
}
