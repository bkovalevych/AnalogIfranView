using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using System.Diagnostics;

namespace AnalogIfranView.IOServices
{
    public class ImageDialogOpener
    {
        private SoftwareBitmap softwareBitmap;
        public async Task<BitmapImage> OpenImageDialog() {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) {
                return null;
            }
            ImageProperties props = await file.Properties.GetImagePropertiesAsync();
            using(IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read)) {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            }
            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read)) {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelHeight = (int)props.Height;
                bitmapImage.DecodePixelWidth = (int)props.Width;
                await bitmapImage.SetSourceAsync(fileStream);
                return bitmapImage;
            }
        }

        public async Task SaveImageDialog() {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPEG images", new List<string>() { ".jpg" });
            picker.FileTypeChoices.Add("PNG images", new List<string>() { ".png" });
            picker.DefaultFileExtension = ".jpg";
            picker.SuggestedFileName = "image";
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            StorageFile storageFile = await picker.PickSaveFileAsync();
            if (storageFile == null) {
                return;
            }
            RenderTargetBitmap render = new RenderTargetBitmap();
            
            // StorageFile src = await StorageFile.GetFileFromApplicationUriAsync(bitmapImage.UriSource);
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite)) {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(SwitchEncoderId(storageFile), stream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                try {
                    await encoder.FlushAsync();
                } catch(Exception e) {
                    Trace.WriteLine(e.Message);
                }
            }

        }

        private async Task saveDrawings() {

        }

        private Guid SwitchEncoderId(StorageFile file) {
            switch(file.FileType) {
                case ".jpg":
                    return BitmapEncoder.JpegEncoderId;
                case ".png":
                    return BitmapEncoder.PngEncoderId;
                default:
                    return BitmapEncoder.JpegEncoderId;
            }
        }
    }
}
