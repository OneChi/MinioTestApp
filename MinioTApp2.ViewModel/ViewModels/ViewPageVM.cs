using MinioTApp2.Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Minio;
using Minio.DataModel;

namespace MinioTApp2.ViewModel.ViewModels
{
    public class ViewPageVM : INotifyPropertyChanged
    {

        // Initialize the client with access credentials.
        private static MinioClient minio;

        public ObservableCollection<Buckets> buckets { get; set; }


        public ViewPageVM() {



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

            buckets = new ObservableCollection<Buckets>();
            //Iterate over the list of buckets.
            foreach (Bucket bucketObj in getListBucketsTask.Result.Buckets)
            {

                //Console.WriteLine(bucketObj.Name + " " + bucketObj.CreationDateDateTime);
                buckets.Add(new Buckets(bucketObj.Name, bucketObj.CreationDate));

            }

            selectedBucket = buckets[0];
            
        }





        private Buckets selectedBucket;
        public Buckets SelectedBucket
        {
            get { return selectedBucket; }
            set
            {
                selectedBucket = value;
                OnPropertyChanged("SelectedBucket");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
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
