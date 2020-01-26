using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinioTApp2.ViewModel.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {

        public ObservableCollection<string> buckets { get; set; }

        public MainWindowVM() {
            buckets = new ObservableCollection<string> {
               "perviy",
               "vtoroy",
               "tretiy"
            };

            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
