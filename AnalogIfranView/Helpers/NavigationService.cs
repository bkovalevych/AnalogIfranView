using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnalogIfranView.Helpers
{
    public sealed class NavigationService
    {
        public void Navigate(Type sourcePage, bool withoutStack = false)
        {
            var frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage);
            if(withoutStack)
            {
                frame.BackStack.RemoveAt(frame.BackStack.Count - 1);
            }
        }

        public void Navigate(Type sourcePage, object parameter)
        {
            var frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage, parameter);
        }

        public void GoBack()
        {
            var frame = (Frame)Window.Current.Content;
            frame.GoBack();
        }


        public ICommand GoBackCommand => new RelayCommand((o) => GoBack());
        public ICommand NavigateCommand => new RelayCommand((o) => Navigate((Type)o));

        private NavigationService()
        {
        }

        private static readonly Lazy<NavigationService> instance =
            new Lazy<NavigationService>(() => new NavigationService());

        public static NavigationService Instance => instance.Value;
    }
}
