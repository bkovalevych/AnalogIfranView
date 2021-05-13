using AnalogIfranView.Services;
using Microsoft.Graphics.Canvas;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;

namespace AnalogIfranView.IOServices
{
    class ImageResizer
    {
        public ImageResizer()
        {

        }

        public void Resize(ref ImageCanvasDataService imageHolst)
        {
            int width = imageHolst.Width;
            int height = imageHolst.Height;
            var softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8,
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

            using(var cds = canvasRenderTarget.CreateDrawingSession())
            {
                cds.DrawImage(canvasBitmap, canvasRenderTarget.Bounds);
            }

            var pixelBytes = canvasRenderTarget.GetPixelBytes();

            imageHolst.ImageSRC.DecodePixelHeight = height;
            imageHolst.ImageSRC.DecodePixelWidth = width;

            var scaledSoftwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, width, height);
            scaledSoftwareBitmap.CopyFromBuffer(pixelBytes.AsBuffer());
            imageHolst.Image = scaledSoftwareBitmap;
        }
    }
}
