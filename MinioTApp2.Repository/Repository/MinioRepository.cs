using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
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
        public ObservableCollection<BucketsMinio> getListBuckets() {
            ObservableCollection<BucketsMinio> buckets = new ObservableCollection<BucketsMinio>();

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
                buckets.Add(new BucketsMinio(bucketObj.Name, bucketObj.CreationDate));
            }
            return buckets;
        }

        public async Task<String> CreateBucketIfExistAsync(String BucketName) {
            try
            {
                // Create bucket if it doesn't exist.
                bool found = await minio.BucketExistsAsync(BucketName);
                if (found)
                {
                    Console.WriteLine("mybucket already exists");
                    return "Exists";
                }
                else
                {
                    // Create bucket 'my-bucketname'.
                    await minio.MakeBucketAsync("mybucket");
                    Console.WriteLine("mybucket is created successfully");
                    return "Secess";
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
                return "Error occurred: " + e;
            }
        }

        public async Task<string> BucketNameExistsAsync(String BucketName) {
            try
            {
                // Check whether 'my-bucketname' exists or not.
                bool found = await minio.BucketExistsAsync(BucketName);

                Console.WriteLine("bucket-name " + ((found == true) ? "exists" : "does not exist"));
                return found ? BucketName : "Not find";
            }
            catch (MinioException e)
            {
                Console.WriteLine("Exception: ", e);
                return "Exception: " + e;
            }
        }
        // Remove bucket BucketName. This operation will succeed only if the bucket is empty.
        public async Task<string> RemoveBucketAsync(String BucketName)
        {
            try
            {
                // Check if my-bucket exists before removing it.
                bool found = await minio.BucketExistsAsync(BucketName);
                if (found)
                {
                    await minio.RemoveBucketAsync(BucketName);
                    Console.WriteLine("mybucket is removed successfully");
                    return "Removed";
                }
                else
                {
                    Console.WriteLine("mybucket does not exist");
                    return "Not Find";
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
                return "Error: " + e;
            }
        }
        // Lists all objects in a bucket or null case error or doesnt exists
        public IObservable<Item> ListObjectsAsync(String BucketName, String Prefix = null, Boolean Recursive = true) 
        {

            try
            {
                // Check whether 'mybucket' exists or not.
                var found = minio.BucketExistsAsync(BucketName);
                Task.WaitAll(found);
                if (found.Result)
                {
                    // List objects from 'my-bucketname'
                    IObservable<Item> observable = minio.ListObjectsAsync(BucketName, Prefix, Recursive);
                    Item A = new Item();
                     
                    return observable;
                    /*IDisposable subscription = observable.Subscribe(
                            item => Console.WriteLine("OnNext: {0}", item.Key),
                            ex => Console.WriteLine("OnError: {0}", ex.Message),
                            () => Console.WriteLine("OnComplete: {0}"));*/
                }
                else
                {
                    Console.WriteLine("mybucket does not exist");
                    return null;
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
                return null;
            }




            /*
                         try
            {
                // Check whether 'mybucket' exists or not.
                bool found = await minio.BucketExistsAsync(BucketName);
                bool complete = false;
                if (found)
                {
                    // List objects from 'my-bucketname'
                    IObservable<Item> observable = minio.ListObjectsAsync(BucketName, Prefix, Recursive);
                    IDisposable subscription = observable.Subscribe(
                            item => itemslist.Add(item),
                            ex => Console.WriteLine("OnError: {0}", ex.Message),
                            () => complete = true);
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine("mybucket does not exist");
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
            }

            return itemslist;
             
             */
        }
        //ListIncompleteUploads
        public async Task<IObservable<Upload>> ListIncompleteUploadsAsync(string bucketName, string prefix, bool recursive) 
        {
            try
            {
                // Check whether 'mybucket' exist or not.
                bool found = await minio.BucketExistsAsync(bucketName);
                if (found)
                {
                    // List all incomplete multipart upload of objects in 'mybucket'
                    IObservable<Upload> observable;
                   return observable = minio.ListIncompleteUploads(bucketName, prefix, recursive);
                    
                    /*IDisposable subscription = observable.Subscribe(
                                        item => Console.WriteLine("OnNext: {0}", item.Key),
                                        ex => Console.WriteLine("OnError: {0}", ex.Message),
                                        () => Console.WriteLine("OnComplete: {0}"));*/
                }
                else
                {
                    Console.WriteLine("mybucket does not exist");
                    return null;
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
                return null;
            }
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
