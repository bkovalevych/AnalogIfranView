﻿using System;
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

    public class Images : Observable {
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
                ScaledWidth = value * Width;
                ScaledHeight = value * Height;
                Set(ref zoom, value);
            }
        }
        private double zoom = 1.0;



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
        }


        public ICommand SaveImageCommand => new RelayCommand(SaveFileFunction);
        private async void SaveFileFunction(object param) {
            await imgOpener.SaveImageDialog(holst);
        }
        private bool isSendedToResize = false;
        public ICommand ResizeCommand => new RelayCommand((o) =>
        {
            isSendedToResize = true;
            NavigationService.Instance.Navigate(typeof(CreatingThumbnailDialog), holst);
        });

        public async Task<bool> Save() {
            bool result = await imgOpener.SaveOnClose(holst);
            return result;
        }

        public ICommand SaveCommand => new RelayCommand(async (o) => await imgOpener.Save(holst));
        private readonly ImageDialogOpener imgOpener;
        private InkStrokeContainer container;
        public Images(InkStrokeContainer container) {
            this.container = container;
            imgOpener = new ImageDialogOpener(container);
            holst = new ThumbnailHolst() { Height = (int)Height, Width = (int)Width, Name = namePicture };
            scaledHeight = height;
            scaledWidth = width;
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
            NamePicture = holst.Name;
        }

        private InkStrokeContainer strokes;
        private IHolst holst;
    }
}
