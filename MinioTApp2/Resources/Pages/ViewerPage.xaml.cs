using MinioTApp2.ViewModel.ViewModels;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace MinioTApp2.Resources.Pages
{
    public sealed partial class ViewerPage : UserControl
    {

        ViewPageVM ViewModel { get; set; }

        public ViewerPage()
        {
            this.InitializeComponent();
            ViewModel = new ViewPageVM();
            DataContext = ViewModel;


        }

        private void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
