using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.Services
{
    public class ImageCanvasDataService : ICanvasDataService
    {
        public int Height
        {
            get; set;
        }
        public int Width
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public SoftwareBitmap Image
        {
            get; set;
        }
        public BitmapImage ImageSRC
        {
            get; set;
        }
        public string FullPath
        {
            get; set;
        }
        public string FutureAccessToken
        {
            get;
            set;
        }

        public async Task<SoftwareBitmap> SaveToBitmap(InkStrokeContainer ink)
        {
            ICanvasResourceCreator device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, Image.PixelWidth, Image.PixelHeight, 96);
            renderTarget.SetPixelBytes(new byte[Image.PixelWidth * 4 * Image.PixelHeight]);
            byte[] imageBytes = new byte[4 * Image.PixelWidth * Image.PixelHeight];
            using(var ds = renderTarget.CreateDrawingSession())
            {
                Image.CopyToBuffer(imageBytes.AsBuffer());
                var win2dRenderedBitmap = CanvasBitmap.CreateFromBytes(
                    device,
                    imageBytes,
                    Image.PixelWidth,
                    Image.PixelHeight,
                    Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    96.0f);
                ds.DrawImage(win2dRenderedBitmap);
                IReadOnlyList<InkStroke> inklist = ink.GetStrokes();
                ds.DrawInk(inklist);
            }
            var inkpixel = renderTarget.GetPixelBytes();
            WriteableBitmap bmp = new WriteableBitmap((int)Image.PixelWidth, (int)Image.PixelHeight);
            Stream s = bmp.PixelBuffer.AsStream();

            s.Seek(0, SeekOrigin.Begin);
            s.Write(inkpixel, 0, (int)Image.PixelWidth * 4 * (int)Image.PixelHeight);
            s.Position = 0;
            await s.ReadAsync(imageBytes, 0, (int)s.Length);
            Image.CopyFromBuffer(imageBytes.AsBuffer());
            return Image;
        }
    }
}
