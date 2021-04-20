using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.IOServices
{
    public class ImageOpener
    {
        public async Task<BitmapImage> OpenImageDialog() {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            
            StorageFile file = await picker.PickSingleFileAsync();
            ImageProperties props = await file.Properties.GetImagePropertiesAsync();
            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read)) {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelHeight = (int)props.Height;
                bitmapImage.DecodePixelWidth = (int)props.Width;

                await bitmapImage.SetSourceAsync(fileStream);
                return bitmapImage;
            }
        }


    }
}
