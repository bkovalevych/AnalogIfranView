using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace AnalogIfranView.Views
{
    using Helpers;
    using Windows.Globalization;
    using ViewModels;
    using CustomProperties;
    using Models;
    using Windows.UI.Core.Preview;

    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.ImagesProp = new Images(inkCanvas.InkPresenter);
            inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            undoRedo = new UndoRedoViewModel(inkCanvas.InkPresenter);
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
            // Ensure the current window is active
            Window.Current.Activate();
        }
        public Images ImagesProp { get; set; }
        public UndoRedoViewModel undoRedo { get; set; }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (e.Parameter is IHolst holst) {
                ImagesProp.InitByHolst(holst);
            }
        }

        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e) {
            var deferral = e.GetDeferral();
            bool shouldClose = await ImagesProp.Save();
            if (shouldClose == false) {
                e.Handled = true;
            }
            deferral.Complete();
        }

    }
}
