using System;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnalogIfranView.Helpers
{
    public class ThemeSwitcher : INotifyPropertyChanged
    {
        public ThemeSwitcher() {
            
        }
        public bool IsLight { 
            set
            {
                ApplicationData.Current.LocalSettings.Values["themSetting"] = value ? 0 : 1;
                (Window.Current.Content as Frame).RequestedTheme = value ? ElementTheme.Light : ElementTheme.Dark;
                OnChanged(nameof(IsLight));
            }
            get
            {
                object val = ApplicationData.Current.LocalSettings.Values["themSetting"];
                if (val != null) {
                    return (int)val == 0;
                }
                int theme = App.Current.RequestedTheme == ApplicationTheme.Light ? 0 : 1;
                ApplicationData.Current.LocalSettings.Values["themSetting"] = theme;
                return theme == 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnChanged(string param) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(param));
        }
        private static readonly Lazy<ThemeSwitcher> instance = new Lazy<ThemeSwitcher>(() => new ThemeSwitcher());
        public ThemeSwitcher Instance => instance.Value;
    }
}
