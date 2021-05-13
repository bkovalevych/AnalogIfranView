using AnalogIfranView.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.IOServices
{
    public class ImageWriterService
    {
        public static async Task<ImageCanvasDataService> StreamToCanvasData(IRandomAccessStream stream, string fileName)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var bitmap = await decoder.GetSoftwareBitmapAsync();
            stream.Seek(0);
            BitmapImage imgSRC = new BitmapImage();
            imgSRC.SetSource(stream);

            return new ImageCanvasDataService()
            {
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                Image = bitmap,
                ImageSRC = imgSRC,
                Name = fileName
            };
        }

        public static async Task<ImageCanvasDataService> StorageFileToCanvasData(StorageFile file)
        {
            using(var stream = await file.OpenReadAsync())
            {
                var result = await StreamToCanvasData(stream, file.DisplayName);
                result.FutureAccessToken = StorageApplicationPermissions.FutureAccessList.Add(file);
                return result;
            }
        }

        public static async Task WriteSoftwareBitmapToFile(StorageFile storageFile, SoftwareBitmap softwareBitmap)
        {
            using(IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(SwitchEncoderId(storageFile.FileType), stream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                try
                {
                    await encoder.FlushAsync();
                }
                catch(Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }
        }

        private static Guid SwitchEncoderId(string fileType)
        {
            switch(fileType)
            {
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
