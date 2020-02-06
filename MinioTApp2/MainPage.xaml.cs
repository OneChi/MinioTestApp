using MinioTApp2.Resources.Pages;
using MinioTApp2.ViewModel.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MinioTApp2
{



    public sealed partial class MainPage : Page
    {
        ViewerPage itemsViewerPage = new ViewerPage();
        public MainPage()
        {
            this.InitializeComponent();
            MainWindowContControl.Content = itemsViewerPage;
            DataContext = new MainWindowVM();


        }


        private void Home_Click(object sender, RoutedEventArgs e) { }
        private void View_Click(object sender, RoutedEventArgs e) { }
        private void Load_Click(object sender, RoutedEventArgs e) { }
    }
}





// (new System.Threading.Thread(delegate () {
//          })).Start(); 

