using System;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnalogIfranView.Helpers
{
    public class ThemeSwitcher : Observable
    {
        private const string THEME_SETTING_KEY = "themeSetting";
        public ThemeSwitcher()
        {
            var defaultTheme = Application.Current.RequestedTheme;
            ApplicationData.Current.LocalSettings.Values.TryGetValue(THEME_SETTING_KEY, out object themeObject);
            var theme = themeObject ?? defaultTheme;
            isLight = (ElementTheme)theme == ElementTheme.Light;
        }

        public bool IsLight
        {
            set
            {
                ApplicationData.Current.LocalSettings.Values[THEME_SETTING_KEY] = value ? 0 : 1;
                (Window.Current.Content as Frame).RequestedTheme = value ? ElementTheme.Light : ElementTheme.Dark;
                Set(ref isLight, value);
            }
            get => isLight;
        }

        private bool isLight;

        private static readonly Lazy<ThemeSwitcher> instance = new Lazy<ThemeSwitcher>(() => new ThemeSwitcher());
        public ThemeSwitcher Instance => instance.Value;
    }
}
