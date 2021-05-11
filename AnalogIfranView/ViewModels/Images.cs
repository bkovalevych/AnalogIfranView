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
    using Views;
    using Windows.ApplicationModel.DataTransfer;
    using System.Numerics;

    public class Images : Observable {
        public string NamePicture
        {
            get => namePicture;
            set => Set(ref namePicture, value);
        }
        private string namePicture = "New picture";
        public float Zoom
        {
            get => zoom;
            set {
                ScaledWidth = Math.Round(value, 1) * Width;
                ScaledHeight = Math.Round(value, 1) * Height;
                //ResizeCanvas(value);
                Set(ref zoom, value);
            }
        }
        private float zoom = 1.0F;
        private void ResizeCanvas(float zoom) {
            var scaleMatrix = Matrix3x2.CreateScale(new Vector2(zoom));

            var resultStrokes = container;
            foreach (var inkStroke in resultStrokes.GetStrokes()) {
                inkStroke.PointTransform = scaleMatrix;
                
                //var da = inkStroke.DrawingAttributes;
                //var daSize = da.Size;
                //daSize.Width = width * zoom;
                //daSize.Height = height * zoom;
                //da.Size = daSize;
                //inkStroke.DrawingAttributes = da;
            }
            
        }
        

        public double ScaledWidth
        {
            get => scaledWidth;
            set => Set(ref scaledWidth, value);
        }
        private double scaledWidth;


        public double ScaledHeight
        {
            get => scaledHeight;
            set => Set(ref scaledHeight, value);
        }
        private double scaledHeight;


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
            Zoom = 1.0F;
            ScaledWidth = Width;
            ScaledHeight = Height;
        }


        public ICommand SaveImageCommand => new RelayCommand(SaveFileFunction);
        private async void SaveFileFunction(object param) {
            await imgOpener.SaveImageDialog(holst, container);
        }
        private bool isSendedToResize = false;
        public ICommand ResizeCommand => new RelayCommand((o) =>
        {
            isSendedToResize = true;
            NavigationService.Instance.Navigate(typeof(CreatingThumbnailDialog), holst);
        });

        public async Task Save() {
            await imgOpener.Save(holst, container);
        }

        public ICommand SaveCommand => new RelayCommand(async (o) => await imgOpener.Save(holst, container));
        private ImageDialogOpener imgOpener;
        private InkStrokeContainer container;
        public InkPresenter Presenter { get => presenter; set 
            {

                container = value.StrokeContainer;
                imgOpener = new ImageDialogOpener();
                Set(ref presenter, value); } }
        private InkPresenter presenter;
        public Images() {
            holst = new ThumbnailHolst() { Height = (int)Height, Width = (int)Width, Name = namePicture };
            scaledHeight = height;
            scaledWidth = width;
        }

        public ICommand InitPresenterCommand => new RelayCommand(InitPresenter);
        private void InitPresenter(object o) {
            if (o is InkPresenter pr) {
                Presenter = pr;
            }
        }

        public ICommand ShareCommand => new RelayCommand(async(o) => {
            Provider provider = new Provider(holst, container);
            await provider.GetRef();
            DataTransferManager.ShowShareUI();
        });

        public InkStrokeContainer Strokes
        {
            get => strokes;
            set => Set(ref strokes, value);
        }

        public void InitByHolst(IHolst holst) {
            if (isSendedToResize) {
                isSendedToResize = false;
                if (holst is ImageHolst imageHolst) {
                    ImageResizer resizer = new ImageResizer();
                    resizer.Resize(ref imageHolst);
                    this.holst = imageHolst;
                }
            } else {
                this.holst = holst;
            }
            Width = holst.Width;
            Height = holst.Height;
            Zoom = 1.0F;
            ScaledWidth = Width;
            scaledHeight = Height;
            NamePicture = holst.Name;
            if (holst is ImageHolst imgHolst) {
                Image = imgHolst.imageSRC;
            }
        }

        private InkStrokeContainer strokes;
        private IHolst holst;
    }
}
