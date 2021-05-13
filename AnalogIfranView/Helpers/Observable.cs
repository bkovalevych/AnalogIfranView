using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnalogIfranView.Helpers
{
    public class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            bool isChanged = false;
            if(!Equals(storage, value))
            {
                isChanged = true;
                storage = value;
                OnPropertyChanged(propertyName);
            }
            return isChanged;
        }
    }
}
