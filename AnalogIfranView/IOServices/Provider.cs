using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.Models
{
    public class Provider
    {
        private DataTransferManager dataTransferManager;
        private IHolst holst;
        private InkStrokeContainer container;
        private SoftwareBitmap softwareBitmap;
        private RandomAccessStreamReference reference;
        public Provider(IHolst holst, InkStrokeContainer container) {
            this.holst = holst;
            this.container = container;
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        public async Task GetRef() {
            softwareBitmap = await holst.SavedBitmap(container);
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, randomAccessStream);
            encoder.SetSoftwareBitmap(softwareBitmap);
            await encoder.FlushAsync();
            randomAccessStream.Seek(0);
            reference = RandomAccessStreamReference.CreateFromStream(randomAccessStream);
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args) {
            DataPackage dataPackage = args.Request.Data;
            dataPackage.Properties.Title = "Sharing image";
            dataPackage.Properties.Description = "Image sharing using AnalogIfranView";
            
            dataPackage.Properties.Thumbnail = reference;
            dataPackage.SetBitmap(reference);
        }
    }
}
