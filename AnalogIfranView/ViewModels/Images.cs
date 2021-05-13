using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using IOServices;
    using Services;
    using System.Numerics;
    using Views;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Input.Inking;

    public class Images : Observable
    {
        private const string DEFAULT_PICTURE_NAME_KEY = "newImageName";
        
        public string NamePicture
        {
            get => namePicture;
            set => Set(ref namePicture, value);
        }

        private string namePicture = ResourceLoader.GetForCurrentView().GetString(DEFAULT_PICTURE_NAME_KEY);
        
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
            if(container != null)
            {
                float this_scale = (float)currentZoom / previousZoom;
                foreach(var inkStroke in container.GetStrokes())
                {
                    var scaleMatrix = Matrix3x2.CreateScale(new Vector2(this_scale * inkStroke.PointTransform.M11));
                    inkStroke.PointTransform = scaleMatrix;
                    var da = inkStroke.DrawingAttributes;
                    var daSize = da.Size;
                    daSize.Width *= this_scale;
                    daSize.Height *= this_scale;
                    da.Size = daSize;
                    inkStroke.DrawingAttributes = da;
                    presenter.UpdateDefaultDrawingAttributes(da);
                }
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
            canvasData = await imgOpener.OpenImageDialog();
            if(canvasData is ImageCanvasDataService data)
            {
                var openedValue = data.ImageSRC;
                Image = openedValue;
                Width = openedValue.DecodePixelWidth;
                Height = openedValue.DecodePixelHeight;
                Zoom = 1.0F;
                ScaledWidth = Width;
                ScaledHeight = Height;
            } 
        }

        public ICommand SaveImageCommand => new RelayCommand(SaveFileFunction);

        private async void SaveFileFunction(object param)
        {
            await imgOpener.SaveImageDialog(canvasData, container);
        }

        private bool isSendedToResize = false;

        public ICommand GoToResizeCommand => new RelayCommand((o) =>
        {
            isSendedToResize = true;
            NavigationService.Instance.Navigate(typeof(CreatingThumbnailDialog), canvasData);
        });

        public async Task Save()
        {
            await imgOpener.Save(canvasData, container);
        }

        public ICommand SaveCommand => new RelayCommand(async (o) => await imgOpener.Save(canvasData, container));

        private ImageDialogOpenerService imgOpener;
        private InkStrokeContainer container;
        
        public InkPresenter Presenter
        {
            get => presenter; set
            {
                container = value.StrokeContainer;
                imgOpener = new ImageDialogOpenerService();
                Set(ref presenter, value);
            }
        }


        private InkPresenter presenter;

        public Images()
        {
            canvasData = new ThumbnailCanvasDataService() { Height = (int)Height, Width = (int)Width, Name = namePicture };
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
            ShareImageProvider provider = new ShareImageProvider(canvasData, container);
            await provider.GetRef();
            DataTransferManager.ShowShareUI();
        });

        public InkStrokeContainer Strokes
        {
            get => strokes;
            set => Set(ref strokes, value);
        }

        public void InitByCanvasData(ICanvasDataService canvasData)
        {
            if(isSendedToResize)
            {
                isSendedToResize = false;
                if(canvasData is ImageCanvasDataService imageCanvasDataVariable)
                {
                    ImageResizer resizer = new ImageResizer();
                    resizer.Resize(ref imageCanvasDataVariable);
                    this.canvasData = imageCanvasDataVariable;
                }
            }
            else
            {
                this.canvasData = canvasData;
            }
            Width = canvasData.Width;
            Height = canvasData.Height;
            Zoom = 1.0F;
            ScaledWidth = Width;
            scaledHeight = Height;
            NamePicture = canvasData.Name;
            if(canvasData is ImageCanvasDataService imageCanvasData)
            {
                Image = imageCanvasData.ImageSRC;
            }
        }

        private InkStrokeContainer strokes;
        private ICanvasDataService canvasData;
    }
}
