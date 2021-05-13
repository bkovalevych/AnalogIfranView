using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.Services
{
    /// <summary>
    /// Base realisation for imageHolst and ThubnailHolst
    /// </summary>
    public interface ICanvasDataService
    {
        int Height
        {
            get; set;
        }
        int Width
        {
            get; set;
        }
        string Name
        {
            get; set;
        }
        string FullPath
        {
            get; set;
        }
        string FutureAccessToken
        {
            get; set;
        }
        Task<SoftwareBitmap> SaveToBitmap(InkStrokeContainer ink);
    }
}
