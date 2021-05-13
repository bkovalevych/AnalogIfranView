using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.Services
{
    public class ShareImageProvider
    {
        private const string SHARED_TARGET_TITLE_KEY = "sharedTargetTitle";
        private const string SHARED_TARGET_DESCRIPTION_KEY = "sharedTargetDescription";
        private readonly DataTransferManager dataTransferManager;
        private readonly ICanvasDataService holst;
        private readonly InkStrokeContainer container;

        private SoftwareBitmap softwareBitmap;
        private RandomAccessStreamReference reference;

        public ShareImageProvider(ICanvasDataService holst, InkStrokeContainer container)
        {
            this.holst = holst;
            this.container = container;
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        public async Task GetRef()
        {
            softwareBitmap = await holst.SaveToBitmap(container);
            var randomAccessStream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, randomAccessStream);
            encoder.SetSoftwareBitmap(softwareBitmap);
            await encoder.FlushAsync();
            randomAccessStream.Seek(0);
            reference = RandomAccessStreamReference.CreateFromStream(randomAccessStream);
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dataPackage = args.Request.Data;
            dataPackage.Properties.Title = ResourceLoader.GetForCurrentView().GetString(SHARED_TARGET_TITLE_KEY);
            dataPackage.Properties.Description = ResourceLoader.GetForCurrentView().GetString(SHARED_TARGET_DESCRIPTION_KEY);

            dataPackage.Properties.Thumbnail = reference;
            dataPackage.SetBitmap(reference);
        }
    }
}
