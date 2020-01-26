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
using MinioTApp2;
using Minio.DataModel;
using Prism.Mvvm;
using Prism.Commands;


namespace MinioTApp2.ViewModel.ViewModels
{
    public class ViewPageVM : BindableBase
    {

        // Initialize the client with access credentials.
        private static MinioClient minio;

        public ObservableCollection<Buckets> buckets { get; set; }

        public DelegateCommand<string> ButtonClickTest  { get; }
      
        

    public ViewPageVM() {

            minio = new MinioClient(
           /*minio server ip =*/                      "83.149.198.59:9000",
           /*minio server open key/ login =*/         "minio",
          /*minio server secret key/ password =*/    "miniominio");
            

            buckets = new ObservableCollection<Buckets>();
            ButtonClickTest = new DelegateCommand<string>(str => {
                /*BUTTON CLICK*/
                buckets.Clear();   
            var rep = App.Repository.getListBuckets();
            foreach (var t in rep)
            buckets.Add(t); //
               
            });
            
            
            /*
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
            //Iterate over the list of buckets.
            foreach (Bucket bucketObj in getListBucketsTask.Result.Buckets)
            {
                //Console.WriteLine(bucketObj.Name + " " + bucketObj.CreationDateDateTime);
                buckets.Add(new Buckets(bucketObj.Name, bucketObj.CreationDate));
            }
            //_selectedBucket = buckets[0];
            */

            
        }
        


        private string _testString;
        public string TestString 
        {
            get { return _testString; }
            set
            { 
                _testString = value;
                RaisePropertyChanged("Test"); 
            }   

        }

        private Buckets _selectedBucket;
        public Buckets SelectedBucket
        {
            get { return _selectedBucket; }
            set
            {
                _selectedBucket = value;
                RaisePropertyChanged("SelectedBucket");
            }
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
