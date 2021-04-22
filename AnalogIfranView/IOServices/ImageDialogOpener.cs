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
        private InkStrokeContainer container;
        public ImageDialogOpener(InkStrokeContainer container) {
            this.container = container; 
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
            await saveDrawings(container);
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
        private async Task<byte[]> EncodedBytes(SoftwareBitmap soft, Guid encoderId) {
            byte[] array = null;

            // First: Use an encoder to copy from SoftwareBitmap to an in-mem stream (FlushAsync)
            // Next:  Use ReadAsync on the in-mem stream to get byte[] array

            using (var ms = new InMemoryRandomAccessStream()) {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, ms);
                encoder.SetSoftwareBitmap(soft);

                try {
                    await encoder.FlushAsync();
                }
                catch (Exception ex) { return new byte[0]; }

                array = new byte[ms.Size];
                await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
            }
            return array;
        }
        private async Task saveDrawings(InkStrokeContainer inkContainer) {
            ICanvasResourceCreator device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, (int)softwareBitmap.PixelWidth, (int)softwareBitmap.PixelHeight, 96);
            renderTarget.SetPixelBytes(new byte[(int)softwareBitmap.PixelWidth * 4 * (int)softwareBitmap.PixelHeight]);
            byte[] imageBytes = new byte[4 * softwareBitmap.PixelWidth * softwareBitmap.PixelHeight];
            using (var ds = renderTarget.CreateDrawingSession()) {
                softwareBitmap.CopyToBuffer(imageBytes.AsBuffer());


                var win2dRenderedBitmap = CanvasBitmap.CreateFromBytes(
                    device,
                    imageBytes,
                    (int)softwareBitmap.PixelWidth,
                    (int)softwareBitmap.PixelHeight,
                    Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    96.0f);
                ds.DrawImage(win2dRenderedBitmap);
                IReadOnlyList<InkStroke> inklist = inkContainer.GetStrokes();
                Debug.WriteLine("Ink_Strokes Count:  " + inklist.Count);
                ds.DrawInk(inklist);
            }
            var inkpixel = renderTarget.GetPixelBytes();
            WriteableBitmap bmp = new WriteableBitmap((int)softwareBitmap.PixelWidth, (int)softwareBitmap.PixelHeight);
            Stream s = bmp.PixelBuffer.AsStream();
            s.Seek(0, SeekOrigin.Begin);
            s.Write(inkpixel, 0, (int)softwareBitmap.PixelWidth * 4 * (int)softwareBitmap.PixelHeight);
            s.Position = 0;
            await s.ReadAsync(imageBytes, 0, (int)s.Length);
            softwareBitmap.CopyFromBuffer(imageBytes.AsBuffer());
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
