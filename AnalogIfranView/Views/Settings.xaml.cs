using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace AnalogIfranView.Views
{
    using Helpers;
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            ThemeSwitcher themeSwitcher = new ThemeSwitcher();
            themeSwitcherUI.DataContext = themeSwitcher;
            LanguageSwitcher languageSwitcher = new LanguageSwitcher();
            languageSwitcherUI.DataContext = languageSwitcher;
        }
    }
}
