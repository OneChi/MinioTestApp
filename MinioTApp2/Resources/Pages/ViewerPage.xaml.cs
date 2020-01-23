using Minio;
using Minio.DataModel;
using MinioTApp2.Resources.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace MinioTApp2.Resources.Pages
{
    public sealed partial class ViewerPage : UserControl
    {


        Buckets focus_1
        {
            get { return focus_1; }
            set
            {
                //TestOut.Text = value.BucketName;
            }
        }

        ObservableCollection<Buckets> buckets = new ObservableCollection<Buckets>();



        // Initialize the client with access credentials.
        private static MinioClient minio;

        public ViewerPage()
        {
            this.InitializeComponent();


            minio = new MinioClient(
           /*minio server ip =*/                      "83.149.198.59:9000",
           /*minio server open key/ login =*/         "minio",
           /*minio server secret key/ password =*/    "miniominio");

            // вызываю в отдельном потоке функцию
            var getListBucketsTask = minio.ListBucketsAsync();




            // Create an async task for listing buckets.
            try
            {
                Task.WaitAll(getListBucketsTask); // block while the task completes
            }
            catch (AggregateException aggEx)
            {
                aggEx.Handle(HandleBatchExceptions);
            }

            // Iterate over the list of buckets.
            foreach (Bucket bucketObj in getListBucketsTask.Result.Buckets)
            {

                //Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
                buckets.Add(new Buckets(bucketObj.Name, bucketObj.CreationDate));

            }
            ListViewOut.ItemsSource = buckets;

        }



        private void ListViewOut_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var buvket = sender as Buckets;
            focus_1 = buvket;

        }

        private void ListViewOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bucket = e.AddedItems[0] as Buckets;
            focus_1 = bucket;

            //TestOut.Text = bucket.Name;
        }


        private static bool HandleBatchExceptions(Exception exceptionToHandle)
        {
            if (exceptionToHandle is ArgumentNullException)
            {
                //I'm handling the ArgumentNullException.
                Console.WriteLine("Handling the ArgumentNullException.");
                //I handled this Exception, return true.
                return true;
            }

            //I'm only handling ArgumentNullExceptions.
            Console.WriteLine(string.Format("I'm not handling the {0}.", exceptionToHandle.GetType()));
            //I didn't handle this Exception, return false.
            return false;
        }

    }
}
