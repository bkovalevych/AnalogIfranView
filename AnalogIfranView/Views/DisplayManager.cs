using System;
using Windows.UI.Xaml.Markup;

namespace AnalogIfranView.Views
{
    [MarkupExtensionReturnType(ReturnType = typeof(Type))]
    public class DisplayManager : MarkupExtension
    {
        public string PathToPage
        {
            set; get;
        }

        private Type GetTypeByPathToPage(string path)
        {
            switch(path)
            {
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

        protected override object ProvideValue()
        {
            return GetTypeByPathToPage(PathToPage);
        }
    }
}
