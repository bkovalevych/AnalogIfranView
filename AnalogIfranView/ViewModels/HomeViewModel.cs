using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using System.Reflection;
    using System.Windows.Input;
    using Views;
    public class HomeViewModel {
        private RelayCommand menuCommand;


        public HomeViewModel() {

        }
        public ICommand NavigateToSettingsPage =>
    new RelayCommand(o => NavigationService.Instance.Navigate(typeof(Settings)));

        

        private static readonly Lazy<HomeViewModel> instance = new Lazy<HomeViewModel>(() => new HomeViewModel());
        public static HomeViewModel Instance => instance.Value;
    }
}
