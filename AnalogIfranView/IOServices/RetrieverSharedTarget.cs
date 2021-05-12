﻿using AnalogIfranView.Models;
using AnalogIfranView.Views;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.IOServices
{
    public static class RetrieverSharedTarget
    {
        public static async Task OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            var shareOperation = args.ShareOperation;
            shareOperation.ReportStarted();
            if(shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                var items = await shareOperation.Data.GetStorageItemsAsync();
                var item = items[0];
                if(item is StorageFile file)
                {
                    using(var stream = await file.OpenReadAsync())
                    {
                        await ProcessStream(stream, shareOperation, file.DisplayName);
                    }
                }
            }
            else if(shareOperation.Data.Contains(StandardDataFormats.Bitmap))
            {
                var reference = await shareOperation.Data.GetBitmapAsync();
                using(var stream = await reference.OpenReadAsync())
                {
                    await ProcessStream(stream, shareOperation, "new File");
                }
            }
        }

        private static async Task ProcessStream(IRandomAccessStream stream, ShareOperation shareOperation, string fileName)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var bitmap = await decoder.GetSoftwareBitmapAsync();
            stream.Seek(0);
            BitmapImage imgSRC = new BitmapImage();
            imgSRC.SetSource(stream);

            ImageCanvasData holst = new ImageCanvasData()
            {
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                Image = bitmap,
                ImageSRC = imgSRC,
                Name = fileName
            };
            shareOperation.ReportDataRetrieved();

            var rootFrame = new Frame();
            Window.Current.Content = rootFrame;
            rootFrame.Navigate(typeof(MainPage), holst);
            Window.Current.Activate();
        }
    }
}
