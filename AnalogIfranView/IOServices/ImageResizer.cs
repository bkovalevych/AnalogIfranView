using AnalogIfranView.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace AnalogIfranView.IOServices
{
    class ImageResizer
    {
        
        public ImageResizer () {
            
        }
        public void Resize(ref ImageHolst imageHolst) {
            int width = imageHolst.Width;
            int height = imageHolst.Height;
            SoftwareBitmap softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, 
                imageHolst.Image.PixelWidth, 
                imageHolst.Image.PixelHeight);
            imageHolst.Image.CopyTo(softwareBitmap);
            byte[] imageBytes = new byte[softwareBitmap.PixelHeight * softwareBitmap.PixelWidth * 4];
            softwareBitmap.CopyToBuffer(imageBytes.AsBuffer());
            var resourceCreator = CanvasDevice.GetSharedDevice();
            var canvasBitmap = CanvasBitmap.CreateFromBytes(
                    resourceCreator,
                    imageBytes,
                    softwareBitmap.PixelWidth,
                    softwareBitmap.PixelHeight,
                    Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    96.0f);
            
            var canvasRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);

            using (var cds = canvasRenderTarget.CreateDrawingSession()) {
                cds.DrawImage(canvasBitmap, canvasRenderTarget.Bounds);
            }

            var pixelBytes = canvasRenderTarget.GetPixelBytes();
            
            imageHolst.imageSRC.DecodePixelHeight = height;
            imageHolst.imageSRC.DecodePixelWidth = width;

            var scaledSoftwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, width, height);
            scaledSoftwareBitmap.CopyFromBuffer(pixelBytes.AsBuffer());
            imageHolst.Image = scaledSoftwareBitmap;
        } 
    }
}
