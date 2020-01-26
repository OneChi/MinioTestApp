using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minio;
using Minio.DataModel;
using MinioTApp2.Model.Models;

namespace MinioTApp2.Repository.Repository
{
    public class MinioRepository
    {


        // Initialize the client with access credentials.
        private static MinioClient minio;

        //public ObservableCollection<Buckets> buckets { get; set; }



        public MinioRepository()
        {
            // TODO: Set Login and Password to access to minio
            LoadMinio();
        }

        // Returns ObservableCollection of "Buckets"(MinioTApp2.Models.Models.Buckets) which stored on remote server
        public ObservableCollection<Buckets> getListBuckets() {
            ObservableCollection<Buckets> buckets = new ObservableCollection<Buckets>();

            // Start function in other thread
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
                buckets.Add(new Buckets(bucketObj.Name, bucketObj.CreationDate));
            }
            return buckets;
        } 

        


        public static void LoadMinio() 
        {
            /*minio server ip =*/
            var MinioServerAddres = "83.149.198.59:9000";
            /*minio server open key/ login =*/
            var MinioLogin = "minio";
            /*minio server secret key/ password =*/
            var MinioPassword = "miniominio";
            // set up minio client
            minio = new MinioClient(MinioServerAddres,MinioLogin,MinioPassword);
        }
        // HANDLING EXCEPTIONS
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
