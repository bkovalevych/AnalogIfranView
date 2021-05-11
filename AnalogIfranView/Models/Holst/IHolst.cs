using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Input.Inking;

namespace AnalogIfranView.Models
{
    /// <summary>
    /// Base realisation for imageHolst and ThubnailHolst
    /// </summary>
    public interface IHolst
    {
        int Height { get; set; }
        int Width { get; set; }
        string Name { get; set; }
        string FullPath { get; set; }
        Task<SoftwareBitmap> SavedBitmap(InkStrokeContainer ink);

    }
}
