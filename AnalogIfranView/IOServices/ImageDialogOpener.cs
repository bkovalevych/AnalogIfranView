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

        private InkStrokeContainer container;
        public ImageDialogOpener(InkStrokeContainer container) {
            this.container = container; 
        }

        public async Task SaveImageDialog(IHolst holst) {
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

        public async Task Save(IHolst holst) {
            if (holst.FullPath == null) {
                await SaveImageDialog(holst);
            }
            else {
                softwareBitmap = await holst.SavedBitmap(container);
                StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(faToken);
                await WriteSoftwareBitmapToFile(file);
            }
        }

        public async Task<bool> SaveOnClose(IHolst holst) {
            var resource = ResourceLoader.GetForCurrentView();
            ContentDialog closeDialog = new ContentDialog {
                Title = resource.GetString("save"),
                Content = resource.GetString("save") + "?",
                CloseButtonText = resource.GetString("cancel"),
                PrimaryButtonText = resource.GetString("yes"),
                SecondaryButtonText = resource.GetString("no"),
            };
            ContentDialogResult result = await closeDialog.ShowAsync();
            switch(result) {
                case ContentDialogResult.Primary:
                    await Save(holst);
                    return true;
                case ContentDialogResult.Secondary:
                    return true;
                default:
                    return false;
            }
        }
    }
}
