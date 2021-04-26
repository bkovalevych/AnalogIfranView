using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using Models;
    using System.Windows.Input;
    using Views;
    using Windows.ApplicationModel.Resources;

    public class ThumbnailCreateViewModel : Observable {
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
        private void SetPortraitSize(object o) {
            Width = 400;
            Height = 800;
        }

        public ICommand SetLandscapeSizeCommand => new RelayCommand(SetLandscapeSize);
        private void SetLandscapeSize(object o) {
            Width = 800;
            Height = 400;
        }

    

        
        private ThumbnailHolst holst;
        private ResourceLoader resource = ResourceLoader.GetForCurrentView();
        public ICommand CreateThumbnailCommand => new RelayCommand(CreateThumbnail);
        private void CreateThumbnail(object o) {
            Validate();
            if (!IsValid) {
                return;
            }
            holst = new ThumbnailHolst() { Height=Height, Width=Width, Name=Name};
            NavigationService.Instance.Navigate(typeof(MainPage), holst);
        }
        
        public void Validate() {
            NameErrorValidation = "";
            WidthErrorValidation = "";
            HeightErrorValidation = "";
            bool isValidLocal = true;
            if (String.IsNullOrEmpty(name)) {
                isValidLocal = false;
                NameErrorValidation = resource.GetString("invalidName");
            }
            if (Width <= 0) {
                isValidLocal = false;
                WidthErrorValidation = resource.GetString("invalidWidth");
            }
            if (Height <= 0) {
                isValidLocal = false;
                HeightErrorValidation = resource.GetString("invalidHeight");
            }
            IsValid = isValidLocal;
        }

        private bool IsValid { set; get; }
    }
}
