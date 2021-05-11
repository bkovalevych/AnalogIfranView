using AnalogIfranView.Models;
using AnalogIfranView.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnalogIfranView.IOServices
{
    public static class FileAssociation
    {
        public static async Task OnFileActivated(FileActivatedEventArgs args) {
            foreach (var f in args.Files) {
                if (f is StorageFile storageFile) {
                    await ProcessFile(storageFile);
                }
                
            }
        }

        private static async Task ProcessFile(StorageFile file) {
            using (var stream = await file.OpenAsync(FileAccessMode.Read)) {
                ImageHolst img = await ImageDialogOpener.StreamToHolst(stream, file.DisplayName);
                CreateNewInstance(img);
            }
        }
        private static void CreateNewInstance(ImageHolst img) {
            Frame rootFrame = new Frame();
            Window.Current.Content = rootFrame;
            rootFrame.Navigate(typeof(MainPage), img);
            Window.Current.Activate();
        }
    }
}
