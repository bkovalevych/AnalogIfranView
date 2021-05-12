using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.IOServices
{
    using Models;

    public class ImageDialogOpener
    {
        private SoftwareBitmap softwareBitmap;
        private string faToken;

        public async Task<ImageCanvasData> OpenImageDialog()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if(file == null)
            {
                return null;
            }
            faToken = StorageApplicationPermissions.FutureAccessList.Add(file);
            var props = await file.Properties.GetImagePropertiesAsync();
            using(IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            }
            using(IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var bitmapImage = new BitmapImage
                {
                    DecodePixelHeight = (int)props.Height,
                    DecodePixelWidth = (int)props.Width
                };
                await bitmapImage.SetSourceAsync(fileStream);
                var result = new ImageCanvasData()
                {
                    FullPath = file.Path,
                    Height = (int)props.Height,
                    Width = (int)props.Width,
                    Image = softwareBitmap,
                    ImageSRC = bitmapImage,
                    Name = file.DisplayName
                };
                return result;
            }
        }

        public ImageDialogOpener()
        {

        }

        public async Task SaveImageDialog(ICanvasData holst, InkStrokeContainer container)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPEG images", new List<string>() { ".jpg" });
            picker.FileTypeChoices.Add("PNG images", new List<string>() { ".png" });
            picker.DefaultFileExtension = ".jpg";
            picker.SuggestedFileName = holst.Name;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var storageFile = await picker.PickSaveFileAsync();
            if(storageFile == null)
            {
                return;
            }
            faToken = StorageApplicationPermissions.FutureAccessList.Add(storageFile);
            holst.FullPath = storageFile.Path;
            softwareBitmap = await holst.SaveToBitmap(container);
            await WriteSoftwareBitmapToFile(storageFile);
        }

        private async Task WriteSoftwareBitmapToFile(StorageFile storageFile)
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

        private Guid SwitchEncoderId(string fileType)
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

        public async Task Save(ICanvasData holst, InkStrokeContainer container)
        {
            if(holst.FullPath == null)
            {
                await SaveImageDialog(holst, container);
            }
            else
            {
                softwareBitmap = await holst.SaveToBitmap(container);
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(faToken);
                await WriteSoftwareBitmapToFile(file);
            }
        }

        public async Task<bool> SaveOnClose(Action savingAction)
        {
            var resource = ResourceLoader.GetForCurrentView();
            var closeDialog = new MessageDialog(resource.GetString("save") + "?");
            bool result = false;
            closeDialog.Commands.Add(new UICommand(
                resource.GetString("yes"),
                new UICommandInvokedHandler((command) =>
                {
                    result = true;
                    savingAction();
                })
                ));
            closeDialog.Commands.Add(new UICommand(
                resource.GetString("no"),
                new UICommandInvokedHandler((command) =>
                {
                    result = true;
                })
                ));
            closeDialog.Commands.Add(new UICommand(
                resource.GetString("cancel"),
                new UICommandInvokedHandler((command) =>
                {
                    result = false;
                })
                ));
            closeDialog.CancelCommandIndex = 2;
            closeDialog.DefaultCommandIndex = 0;
            await closeDialog.ShowAsync();
            return result;
        }

        public static async Task<ImageCanvasData> StreamToHolst(IRandomAccessStream stream, string fileName)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var bitmap = await decoder.GetSoftwareBitmapAsync();
            stream.Seek(0);
            var imgSRC = new BitmapImage();
            imgSRC.SetSource(stream);

            var canvasData = new ImageCanvasData()
            {
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                Image = bitmap,
                ImageSRC = imgSRC,
                Name = fileName
            };
            return canvasData;
        }
    }
}
