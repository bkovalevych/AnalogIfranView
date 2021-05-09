﻿using System;
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
        private HomeViewModel vm;
        public MainPage()
        {
            this.InitializeComponent();
            vm = new HomeViewModel();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
            Window.Current.Activate();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (e.Parameter is IHolst holst) {
                vm.SelectedPage?.Images.InitByHolst(holst);
            }
        }

        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e) {
            var deferral = e.GetDeferral();
            bool shouldClose = await vm.SaveOnClose();
            if (shouldClose == false) {
                e.Handled = true;
            }
            deferral.Complete();
        }

    }
}
