using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AnalogIfranView.ViewModels
{
    using Helpers;
    using IOServices;
   
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
            get => selectedindex; set => Set(ref selectedindex, value);
        }
        
        private int selectedindex = -1;

        private readonly ImageDialogOpener opener;

        public ICommand AddPage => new RelayCommand((o) =>
        {
            PageHolder new_val = new PageHolder();
            Pages.Add(new_val);
            SelectedIndex = Pages.Count - 1;
            SelectedPage.Images.NamePicture += Pages.Count.ToString();
        });
        
        public HomeViewModel()
        {
            opener = new ImageDialogOpener();
            Pages = new ObservableCollection<PageHolder>();
            var now = new PageHolder();
            Pages.Add(now);
            SelectedPage = now;
        }

        public async Task<bool> SaveOnClose()
        {
            bool result = await opener.SaveOnClose(async () =>
            {
                foreach(var page in Pages)
                {
                    await page.Images.Save();
                }
            });
            return result;
        }
    }
}
