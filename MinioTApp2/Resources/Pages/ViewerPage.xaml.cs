using Minio;
using Minio.DataModel;
using MinioTApp2.Model.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MinioTApp2.ViewModel.ViewModels;

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace MinioTApp2.Resources.Pages
{
    public sealed partial class ViewerPage : UserControl
    {

        ViewPageVM ViewModel { get; set; }

        public ViewerPage()
        {
            this.InitializeComponent();
            ViewModel =  new ViewPageVM();
            DataContext = ViewModel;


        }



        private void ListViewOut_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }

        private void ListViewOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bucket = e.AddedItems[0] as Buckets;
           //focus_1 = bucket;

            //TestOut.Text = bucket.Name;
        }


    }
}
