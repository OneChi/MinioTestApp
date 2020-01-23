using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Minio;
using Minio.DataModel;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using MinioTApp2.Resources.Pages;
using MUXC = Microsoft.UI.Xaml.Controls;


using System.Collections.ObjectModel;
using MinioTApp2.Resources.Classes;

namespace MinioTApp2
{



    public sealed partial class MainPage : Page
    {



        // Initialize the client with access credentials.
        private static MinioClient minio;
        // переменная для отображения крутилки-прогрессбара
        private bool isWorkInProgress
        {
            get { return isWorkInProgress;}
           set
            {
                if (value == false)
                    workRing.Visibility = Visibility.Collapsed;
                else if (value == true)

                    workRing.Visibility = Visibility.Visible;
            }
        }
        //фокус в главном окне
        Buckets focus_1 {
            get { return focus_1; }
            set
            {
                TestOut.Text = value.BucketName;
            }
        }

        ViewerPage itemsViewerPage = new ViewerPage();


        ObservableCollection<Buckets> buckets = new ObservableCollection<Buckets>();



        public MainPage()
        {
            this.InitializeComponent();
            // Create an async task for listing buckets.

            minio = new MinioClient(
           /*minio server ip =*/                      "83.149.198.59:9000",
           /*minio server open key/ login =*/         "minio",
           /*minio server secret key/ password =*/    "miniominio");

            // вызываю в отдельном потоке функцию
            var getListBucketsTask = minio.ListBucketsAsync();
            
            MainWindowContControl.Content = itemsViewerPage;



                try
                {
                isWorkInProgress = true;
                    Task.WaitAll(getListBucketsTask); // block while the task completes
                isWorkInProgress = false;
                }
                catch (AggregateException aggEx)
                {
                    aggEx.Handle(HandleBatchExceptions);
                isWorkInProgress = false;
                }

                // Iterate over the list of buckets.
                foreach (Bucket bucketObj in getListBucketsTask.Result.Buckets)
                {

                //Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
                buckets.Add(new Buckets(bucketObj.Name, bucketObj.CreationDate));

                TestOut.Text = "";
                TestOut.Text = TestOut.Text + buckets.Last().BucketName;
                    
                }
            ListViewOut.ItemsSource = buckets;





            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Input.StandardUICommand"))
            {
                var cutCommand = new StandardUICommand(StandardUICommandKind.Cut);
                cutCommand.ExecuteRequested += CutCommand_ExecuteRequested;
                CutItem.Command = cutCommand;

            }

            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Input.KeyboardAccelerator"))
            {
                var accelerator = new KeyboardAccelerator();
                accelerator.Modifiers = Windows.System.VirtualKeyModifiers.Control;
                accelerator.Key = Windows.System.VirtualKey.Z;
                UndoItem.KeyboardAccelerators.Add(accelerator);
            }
            UndoItem.Click += UndoItem_Click;



        }


   


        private void UndoItem_Click(object sender, RoutedEventArgs e)
        {
           TestOut.Text = "Undo Clicked";
        }

        private void CutCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            TestOut.Text = "Cut Clicked";
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

        private void MenuOpenItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void MenuSaveItem_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
     




        // (new System.Threading.Thread(delegate () {
        //          })).Start(); 

