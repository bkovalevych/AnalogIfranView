using System;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using Models;
    using Views;

    public class ThumbnailCreateViewModel : Observable
    {
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
            Width = 400;
            Height = 800;
        }

        public ICommand SetLandscapeSizeCommand => new RelayCommand(SetLandscapeSize);
        
        private void SetLandscapeSize(object o)
        {
            Width = 800;
            Height = 400;
        }

        private ICanvasData holst;
        private readonly ResourceLoader resource = ResourceLoader.GetForCurrentView();
        
        public ICommand CreateThumbnailCommand => new RelayCommand(CreateThumbnail);
        
        private void CreateThumbnail(object o)
        {
            Validate();
            if(!IsValid)
            {
                return;
            }
            if(creatingMode)
            {
                holst = new ThumbnailCanvasData() { Height = Height, Width = Width, Name = Name };
            }
            else
            {
                holst.Height = Height;
                holst.Width = Width;
            }
            NavigationService.Instance.Navigate(typeof(MainPage), holst);
        }

        public bool CreatingMode
        {
            set => Set(ref creatingMode, value); get => creatingMode;
        }

        private bool creatingMode = true;

        public void InitByHolst(ICanvasData holst)
        {
            this.holst = holst;
            Width = holst.Width;
            Height = holst.Height;
            Name = holst.Name;
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
