using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using IOServices;
    using System.Collections.Generic;
    using Windows.UI.Input.Inking;

    public class HomeViewModel : Observable
    {
        public ObservableCollection<PageHolder> Pages
        {
            get; set;
        }

        public PageHolder SelectedPage
        {
            get => selectedPage; set => Set(ref selectedPage, value);
        }

        private PageHolder selectedPage;
        
        public int SelectedIndex
        {
            get => selectedindex; set
            {
                if(value != -1)
                {
                    SelectedPage = Pages[value];
                }
                Set(ref selectedindex, value); 
            }
        }
        
        private int selectedindex = -1;

        private readonly ImageDialogOpenerService opener;

        public ICommand AddPage => new RelayCommand((o) =>
        {
            PageHolder new_val = new PageHolder();
            Pages.Add(new_val);
            SelectedIndex = Pages.Count - 1;
            SelectedPage.ThumbnailViewModel.NamePicture += " " + Pages.Count.ToString();
        });

        private readonly Dictionary<int, InkPresenter> dict;

        public ICommand DeletePage => new RelayCommand((o) =>
        {
            if(o is PageHolder pageHolder)
            {
                dict[Pages.IndexOf(pageHolder)] = pageHolder.Presenter;
                Pages.Remove(pageHolder);
            }
        });

        public HomeViewModel()
        {
            opener = new ImageDialogOpenerService();
            Pages = new ObservableCollection<PageHolder>();
            var now = new PageHolder();
            dict = new Dictionary<int, InkPresenter>();
            Pages.Add(now);
            SelectedPage = now;
        }

        public async Task<bool> SaveOnClose()
        {
            bool result = await opener.SaveOnClose(async () =>
            {
                foreach(var page in Pages)
                {
                    await page.ThumbnailViewModel.Save();
                }
            });
            return result;
        }
    }
}
