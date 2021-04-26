using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Markup;

namespace AnalogIfranView.Views
{
    using Helpers;
   

    [MarkupExtensionReturnType(ReturnType = typeof(Type))]
    public class DisplayManager : MarkupExtension {
        public string PathToPage { set; get; }
        private Type PathToFunc(string path) {
            switch (path) {
                case nameof(MainPage):
                    return typeof(MainPage);
                case nameof(Settings):
                    return typeof(Settings);
                case nameof(CreatingThumbnailDialog):
                    return typeof(CreatingThumbnailDialog);
                default:
                    return typeof(MainPage);
            }
        }
        protected override object ProvideValue() {
            return PathToFunc(PathToPage);
        }
    }
}
