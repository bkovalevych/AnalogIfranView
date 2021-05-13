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
    public class ThumbnailCanvasDataService : ICanvasDataService
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
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, Width, Height, 96);
            var src = new byte[Width * 4 * Height];
            for(int index = 0; index < src.Length; ++index)
            {
                src[index] = 255;
            }
            renderTarget.SetPixelBytes(src);
            byte[] imageBytes = new byte[4 * Width * Height];
            using(var ds = renderTarget.CreateDrawingSession())
            {
                IReadOnlyList<InkStroke> inklist = ink.GetStrokes();
                ds.DrawInk(inklist);
            }
            var inkpixel = renderTarget.GetPixelBytes();
            WriteableBitmap bmp = new WriteableBitmap(Width, Height);
            Stream s = bmp.PixelBuffer.AsStream();
            s.Seek(0, SeekOrigin.Begin);
            s.Write(inkpixel, 0, Width * 4 * Height);
            s.Position = 0;
            await s.ReadAsync(imageBytes, 0, (int)s.Length);
            SoftwareBitmap result = new SoftwareBitmap(BitmapPixelFormat.Bgra8, Width, Height, BitmapAlphaMode.Premultiplied);
            result.CopyFromBuffer(imageBytes.AsBuffer());
            return result;
        }
    }
}
