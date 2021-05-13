using System;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using Services;
    using Views;

    public class ThumbnailCreateViewModel : Observable
    {
        private const int STANDART_WIDTH_LANDSCAPE = 800;
        private const int STANDART_HEIGHT_LANDSCAPE = 400;

        public int Width
        {
            get => width;
            set => Set(ref width, value);
        }

        private int width;
        
        public int Height
        {
            get => height;
            set => Set(ref height, value);
        }
        
        private int height;

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
        
        private string name;

        public string NameErrorValidation
        {
            get => nameErrorValidation;
            set => Set(ref nameErrorValidation, value);
        }
        
        private string nameErrorValidation;

        public string WidthErrorValidation
        {
            get => widthErrorValidation;
            set => Set(ref widthErrorValidation, value);
        }
        
        private string widthErrorValidation;

        public string HeightErrorValidation
        {
            get => heightErrorValidation;
            set => Set(ref heightErrorValidation, value);
        }
        
        private string heightErrorValidation;

        public ICommand SetPortraitSizeCommand => new RelayCommand(SetPortraitSize);
        
        private void SetPortraitSize(object o)
        {
            Width = STANDART_HEIGHT_LANDSCAPE;
            Height = STANDART_WIDTH_LANDSCAPE;
        }

        public ICommand SetLandscapeSizeCommand => new RelayCommand(SetLandscapeSize);
        
        private void SetLandscapeSize(object o)
        {
            Width = STANDART_WIDTH_LANDSCAPE;
            Height = STANDART_HEIGHT_LANDSCAPE;
        }

        private ICanvasDataService canvasData;
        private readonly ResourceLoader resource = ResourceLoader.GetForCurrentView();
        
        public ICommand CreateThumbnailCommand => new RelayCommand(CreateThumbnail);
        
        private void CreateThumbnail(object o)
        {
            Validate();
            if(IsValid)
            {
                if(creatingMode)
                {
                    canvasData = new ThumbnailCanvasDataService() { 
                        Height = Height, 
                        Width = Width, 
                        Name = Name };
                }
                else
                {
                    canvasData.Height = Height;
                    canvasData.Width = Width;
                }
            }
            NavigationService.Instance.Navigate(typeof(MainPage), canvasData);
        }

        public bool CreatingMode
        {
            set => Set(ref creatingMode, value); get => creatingMode;
        }

        private bool creatingMode = true;

        public void InitByHolst(ICanvasDataService canvasData)
        {
            this.canvasData = canvasData;
            Width = canvasData.Width;
            Height = canvasData.Height;
            Name = canvasData.Name;
            CreatingMode = false;
        }

        public void Validate()
        {
            NameErrorValidation = "";
            WidthErrorValidation = "";
            HeightErrorValidation = "";
            bool isValidLocal = true;
            if(String.IsNullOrEmpty(name))
            {
                isValidLocal = false;
                NameErrorValidation = resource.GetString("invalidName");
            }
            if(Width <= 0)
            {
                isValidLocal = false;
                WidthErrorValidation = resource.GetString("invalidWidth");
            }
            if(Height <= 0)
            {
                isValidLocal = false;
                HeightErrorValidation = resource.GetString("invalidHeight");
            }
            IsValid = isValidLocal;
        }

        private bool IsValid
        {
            set; get;
        }
    }
}
