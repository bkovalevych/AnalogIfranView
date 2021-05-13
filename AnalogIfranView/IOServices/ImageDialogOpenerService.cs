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
    using Services;

    public class ImageDialogOpenerService
    {
        public async Task<ImageCanvasDataService> OpenImageDialog()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            ImageCanvasDataService result = null;
            if(file != null)
            {
                result = await ImageWriterService.StorageFileToCanvasData(file);
            }
            return result;
        }


        public ImageDialogOpenerService()
        {

        }

        public async Task SaveImageDialog(ICanvasDataService canvasData, InkStrokeContainer container)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPEG images", new List<string>() { ".jpg" });
            picker.FileTypeChoices.Add("PNG images", new List<string>() { ".png" });
            picker.DefaultFileExtension = ".jpg";
            picker.SuggestedFileName = canvasData.Name;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var storageFile = await picker.PickSaveFileAsync();
            if(storageFile != null)
            {
                canvasData.FullPath = storageFile.Path;
                canvasData.FutureAccessToken = StorageApplicationPermissions.FutureAccessList.Add(storageFile);
                var softwareBitmap = await canvasData.SaveToBitmap(container);
                await ImageWriterService.WriteSoftwareBitmapToFile(storageFile, softwareBitmap);
            }
        }

        


        public async Task Save(ICanvasDataService canvasData, InkStrokeContainer container)
        {
            if(canvasData.FullPath == null)
            {
                await SaveImageDialog(canvasData, container);
            }
            else
            {
                var softwareBitmap = await canvasData.SaveToBitmap(container);
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(canvasData.FutureAccessToken);
                await ImageWriterService.WriteSoftwareBitmapToFile(file, softwareBitmap);
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
    }
}
