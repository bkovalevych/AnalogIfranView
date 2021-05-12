using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using IOServices;
    using Models;
    using System.Numerics;
    using Views;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.UI.Input.Inking;

    public class Images : Observable
    {
        public string NamePicture
        {
            get => namePicture;
            set => Set(ref namePicture, value);
        }
        private string namePicture = "New picture";
        public float Zoom
        {
            get => zoom;
            set
            {
                ScaledWidth = Math.Round(value, 1) * Width;
                ScaledHeight = Math.Round(value, 1) * Height;
                ResizeCanvas(value, zoom);
                Set(ref zoom, value);
            }
        }
        private float zoom = 1.0F;
        private void ResizeCanvas(float currentZoom, float previousZoom)
        {

            var scaleMatrix = Matrix3x2.CreateScale(new Vector2(currentZoom));
            foreach(var inkStroke in container.GetStrokes())
            {
                inkStroke.PointTransform = scaleMatrix;
                var da = inkStroke.DrawingAttributes;
                var daSize = da.Size;
                var difference = currentZoom - previousZoom;
                daSize.Width *= 1 +  difference;
                daSize.Height *= 1 + difference;
                da.Size = daSize;
                inkStroke.DrawingAttributes = da;
                presenter.UpdateDefaultDrawingAttributes(da);
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
        
        private async void OpenFileFunction(object param)
        {
            holst = await imgOpener.OpenImageDialog();
            BitmapImage openedValue = ((ImageCanvasData)holst).ImageSRC;
            if(openedValue == null)
            {
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

        private async void SaveFileFunction(object param)
        {
            await imgOpener.SaveImageDialog(holst, container);
        }

        private bool isSendedToResize = false;

        public ICommand ResizeCommand => new RelayCommand((o) =>
        {
            isSendedToResize = true;
            NavigationService.Instance.Navigate(typeof(CreatingThumbnailDialog), holst);
        });

        public async Task Save()
        {
            await imgOpener.Save(holst, container);
        }

        public ICommand SaveCommand => new RelayCommand(async (o) => await imgOpener.Save(holst, container));

        private ImageDialogOpener imgOpener;
        private InkStrokeContainer container;
        
        public InkPresenter Presenter
        {
            get => presenter; set
            {
                container = value.StrokeContainer;
                value.StrokesCollected += Value_StrokesCollected;
                imgOpener = new ImageDialogOpener();
                Set(ref presenter, value);
            }
        }

        private void Value_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            foreach(var stroke in args.Strokes)
            {
                //stroke.PointTransform = Matrix3x2.CreateScale(new Vector2(1 /zoom));
                //stroke.PointTransform = Matrix3x2.CreateTranslation((float)stroke.BoundingRect.X * (1 / zoom - 1), (float)stroke.BoundingRect.Y * (1 / zoom - 1));
            }
        }

        private InkPresenter presenter;

        public Images()
        {
            holst = new ThumbnailCanvasData() { Height = (int)Height, Width = (int)Width, Name = namePicture };
            scaledHeight = height;
            scaledWidth = width;
        }

        public ICommand InitPresenterCommand => new RelayCommand(InitPresenter);
        
        private void InitPresenter(object o)
        {
            if(o is InkPresenter pr)
            {
                Presenter = pr;
            }
        }

        public ICommand ShareCommand => new RelayCommand(async (o) =>
        {
            Provider provider = new Provider(holst, container);
            await provider.GetRef();
            DataTransferManager.ShowShareUI();
        });

        public InkStrokeContainer Strokes
        {
            get => strokes;
            set => Set(ref strokes, value);
        }

        public void InitByHolst(ICanvasData holst)
        {
            if(isSendedToResize)
            {
                isSendedToResize = false;
                if(holst is ImageCanvasData imageHolst)
                {
                    ImageResizer resizer = new ImageResizer();
                    resizer.Resize(ref imageHolst);
                    this.holst = imageHolst;
                }
            }
            else
            {
                this.holst = holst;
            }
            Width = holst.Width;
            Height = holst.Height;
            Zoom = 1.0F;
            ScaledWidth = Width;
            scaledHeight = Height;
            NamePicture = holst.Name;
            if(holst is ImageCanvasData imgHolst)
            {
                Image = imgHolst.ImageSRC;
            }
        }

        private InkStrokeContainer strokes;
        private ICanvasData holst;
    }
}
