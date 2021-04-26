using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;

namespace AnalogIfranView.ViewModels
{
    [MarkupExtensionReturnType(ReturnType = typeof(double))]
    public class Scaler : MarkupExtension
    {
        public object Size { set; get; }
        public object Zoom { set; get; }
        protected override object ProvideValue() {
            return (double)Size * (double)Zoom;
        }

    }
}
