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
using Windows.UI.Input.Inking;
using Microsoft.Graphics.Canvas;

namespace AnalogIfranView.IOServices
{
    using Models;
    using Windows.ApplicationModel.Resources;
    using Windows.Storage.AccessCache;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Controls;

    public class ImageDialogOpener
    {
        private SoftwareBitmap softwareBitmap;
        private string faToken;
        public async Task<ImageHolst> OpenImageDialog() {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) {
                return null;
            }
            faToken = StorageApplicationPermissions.FutureAccessList.Add(file);
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
                ImageHolst result = new ImageHolst() {
                    FullPath = file.Path,
                    Height = (int)props.Height,
                    Width = (int)props.Width,
                    Image = softwareBitmap,
                    imageSRC = bitmapImage,
                    Name = file.DisplayName
                };
                return result;
            }
        }

       
        public ImageDialogOpener() {
             
        }

        public async Task SaveImageDialog(IHolst holst, InkStrokeContainer container) {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPEG images", new List<string>() { ".jpg" });
            picker.FileTypeChoices.Add("PNG images", new List<string>() { ".png" });
            picker.DefaultFileExtension = ".jpg";
            picker.SuggestedFileName = holst.Name;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            StorageFile storageFile = await picker.PickSaveFileAsync();
            if (storageFile == null) {
                return;
            }
            faToken = StorageApplicationPermissions.FutureAccessList.Add(storageFile);
            holst.FullPath = storageFile.Path;
            softwareBitmap = await holst.SavedBitmap(container);
            await WriteSoftwareBitmapToFile(storageFile);
        }

        private async Task WriteSoftwareBitmapToFile(StorageFile storageFile) {
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite)) {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(SwitchEncoderId(storageFile.FileType), stream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                try {
                    await encoder.FlushAsync();
                }
                catch (Exception e) {
                    Trace.WriteLine(e.Message);
                }
            }
        }
        

        private Guid SwitchEncoderId(string fileType) {
            switch(fileType) {
                case ".jpg":
                    return BitmapEncoder.JpegEncoderId;
                case ".png":
                    return BitmapEncoder.PngEncoderId;
                default:
                    return BitmapEncoder.JpegEncoderId;
            }
        }

        public async Task Save(IHolst holst, InkStrokeContainer container) {
            if (holst.FullPath == null) {
                 await SaveImageDialog(holst, container);
            }
            else {
                softwareBitmap = await holst.SavedBitmap(container);
                StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(faToken);
                await WriteSoftwareBitmapToFile(file);
            }
        }

        public async Task<bool> SaveOnClose(Action savingAction) {
            var resource = ResourceLoader.GetForCurrentView();
            MessageDialog closeDialog = new MessageDialog(resource.GetString("save") + "?");
            bool result = false;
            closeDialog.Commands.Add(new UICommand(
                resource.GetString("yes"),
                new UICommandInvokedHandler((command) => {
                    result = true;
                    savingAction();
                    })
                ));
            closeDialog.Commands.Add(new UICommand(
                resource.GetString("no"),
                new UICommandInvokedHandler((command) => {
                    result = true;
                })
                ));
            closeDialog.Commands.Add(new UICommand(
                resource.GetString("cancel"),
                new UICommandInvokedHandler((command) => {
                    result = false;
                })
                ));
            closeDialog.CancelCommandIndex = 2;
            closeDialog.DefaultCommandIndex = 0;
            await closeDialog.ShowAsync();
            return result;
        }

        public static async Task<ImageHolst> StreamToHolst(IRandomAccessStream stream, string fileName) {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync();
            stream.Seek(0);
            BitmapImage imgSRC = new BitmapImage();
            imgSRC.SetSource(stream);

            ImageHolst holst = new ImageHolst() {
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                Image = bitmap,
                imageSRC = imgSRC,
                Name = fileName
            };
            return holst;
        }
    }
}
