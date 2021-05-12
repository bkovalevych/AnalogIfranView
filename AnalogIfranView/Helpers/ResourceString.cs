using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Markup;

namespace AnalogIfranView.Helpers
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ResourceString : MarkupExtension
    {
        private static readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        public string Name
        {
            get; set;
        }

        protected override object ProvideValue()
        {
            return resourceLoader.GetString(this.Name);
        }
    }
}
