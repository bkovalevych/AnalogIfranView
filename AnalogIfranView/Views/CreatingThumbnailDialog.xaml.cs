using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace AnalogIfranView.Views
{
    using Services;
    using ViewModels;

    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CreatingThumbnailDialog : Page
    {
        public ThumbnailCreateViewModel vmField;

        public CreatingThumbnailDialog()
        {
            this.InitializeComponent();
            vmField = new ThumbnailCreateViewModel();
        }        
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is ICanvasDataService canvasData)
            {
                vmField.InitByHolst(canvasData);
            }
        }
    }
}
