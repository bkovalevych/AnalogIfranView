using AnalogIfranView.Services;
using AnalogIfranView.Views;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.IOServices
{
    public class SharedTargetService
    {
        private const string NEW_IMAGE_NAME_KEY = "newImageName";
        public static async Task OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            var shareOperation = args.ShareOperation;
            shareOperation.ReportStarted();
            if(shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                var items = await shareOperation.Data.GetStorageItemsAsync();
                foreach(var item in items)
                {
                    if(item is StorageFile file)
                    {
                        var canvasData = await ImageWriterService.StorageFileToCanvasData(file);
                        shareOperation.ReportDataRetrieved();
                        NavigateToMain(canvasData);   
                    }
                }
            }
            else if(shareOperation.Data.Contains(StandardDataFormats.Bitmap))
            {
                var reference = await shareOperation.Data.GetBitmapAsync();
                using(var stream = await reference.OpenReadAsync())
                {
                    await ProcessSharedImage(
                        stream, 
                        shareOperation, 
                        ResourceLoader.GetForCurrentView().GetString(NEW_IMAGE_NAME_KEY));
                }
            }
        }

        private static async Task ProcessSharedImage(IRandomAccessStream stream, ShareOperation operation, string fileName)
        {
            var canvasData = await ImageWriterService.StreamToCanvasData(stream, fileName);
            operation.ReportDataRetrieved();
            NavigateToMain(canvasData);
        }

        


        private static void NavigateToMain(ICanvasDataService canvasData)
        {
            if(!(Window.Current.Content is Frame))
            {
                Window.Current.Content = new Frame();
            }
            var frame = Window.Current.Content as Frame;
            Window.Current.Content = frame;
            frame.Navigate(typeof(MainPage), canvasData);
            Window.Current.Activate();
        }

        public static async Task OnFileActivated(FileActivatedEventArgs args)
        {
            foreach(var f in args.Files)
            {
                if(f is StorageFile storageFile)
                {
                    using(var stream = await storageFile.OpenReadAsync())
                    {
                        await ImageWriterService.StreamToCanvasData(stream, storageFile.DisplayName);
                    }
                }
            }
        }
    }
}
