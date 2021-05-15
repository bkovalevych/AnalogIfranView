using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace AnalogIfranView.Views
{
    using Services;
    using ViewModels;
    using Windows.UI.Core.Preview;

    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly HomeViewModel vm;
        public MainPage()
        {
            this.InitializeComponent();
            vm = new HomeViewModel();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
            Window.Current.Activate();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is ICanvasDataService holst)
            {
                vm.SelectedPage?.ThumbnailViewModel.InitByCanvasData(holst);
            }
        }

        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
            bool shouldClose = await vm.SaveOnClose();
            if(shouldClose == false)
            {
                e.Handled = true;
            }
            deferral.Complete();
        }

    }
}
