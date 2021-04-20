using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace AnalogIfranView.Helpers
{
    public class LanguageSwitcher : INotifyPropertyChanged
    {
        private string currentLanguage;
        public string CurrentLanguage
        {
            get => currentLanguage;
            set
            {
                currentLanguage = value;
                var culture = new System.Globalization.CultureInfo(currentLanguage);
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture.Name;
                Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();
                Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
                NavigationService.Instance.Navigate(typeof(Views.Settings), true);
                OnChanged(nameof(CurrentLanguage));
            }
        }
        
        public IReadOnlyList<string> Languages
        {
            get => Windows.Globalization.ApplicationLanguages.ManifestLanguages;
        }
        

        public LanguageSwitcher() {
            object val = ApplicationData.Current.LocalSettings.Values["language"];
            if (val != null) {
                currentLanguage = (string)val;
            } else {
                currentLanguage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
            }
        }
       

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnChanged(string param) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(param));
        }

        private static readonly Lazy<LanguageSwitcher> instance = new Lazy<LanguageSwitcher>(() => new LanguageSwitcher());
        public LanguageSwitcher Instance => instance.Value;
    }
}
