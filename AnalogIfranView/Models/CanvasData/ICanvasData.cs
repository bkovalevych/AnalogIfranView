using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.Models
{
    /// <summary>
    /// Base realisation for imageHolst and ThubnailHolst
    /// </summary>
    public interface ICanvasData
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

        Task<SoftwareBitmap> SaveToBitmap(InkStrokeContainer ink);
    }
}
