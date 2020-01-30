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
        public ObservableCollection<MinioBucketModel> getListBuckets() {
            ObservableCollection<MinioBucketModel> buckets = new ObservableCollection<MinioBucketModel>();

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
                buckets.Add(new MinioBucketModel(bucketObj.Name, bucketObj.CreationDate));
            }
            return buckets;
        }

        // true - bucket was created | false - bucket already exists
        public async Task<Boolean> CreateBucketIfExistAsync(String BucketName) {
            try
            {
                // Create bucket if it doesn't exist.
                bool found = await minio.BucketExistsAsync(BucketName);
                if (found)
                {
                    return false;
                }
                else
                {
                    // Create bucket 'bucketname'.
                    await minio.MakeBucketAsync(BucketName);
                    return true;
                }
            }
            catch (MinioException e){throw;}
        }
        // true - bucket found false - bucket not found
        public async Task<Boolean> BucketNameExistsAsync(String BucketName) {
            try
            {
                // Check whether 'my-bucketname' exists or not.
                bool found = await minio.BucketExistsAsync(BucketName);
                return found ? true : false;
            }
            catch (MinioException e)
            {
                throw; 
            }
        }
        // Remove bucket BucketName. This operation will succeed only if the bucket is empty.
        public async Task<Boolean> RemoveBucketAsync(String BucketName)
        {
            // TODO handle situation when bucket are not empty
            try
            {
                // Check if my-bucket exists before removing it.
                bool found = await minio.BucketExistsAsync(BucketName);
                if (found)
                {
                    await minio.RemoveBucketAsync(BucketName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MinioException e)
            {
                throw;
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
                    return observable;
                    /*IDisposable subscription = observable.Subscribe(
                            item => Console.WriteLine("OnNext: {0}", item.Key),
                            ex => Console.WriteLine("OnError: {0}", ex.Message),
                            () => Console.WriteLine("OnComplete: {0}"));*/
                }
                else
                {
                    throw new BucketNotFoundException();
                }
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //ListIncompleteUploads
        public IObservable<Upload> ListIncompleteUploadsAsync(string bucketName, string prefix, bool recursive) 
        {
            try
            {
                // Check whether 'mybucket' exist or not.
                var found = minio.BucketExistsAsync(bucketName);
                Task.WaitAll(found);
                if (found.Result)
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
                    throw new Minio.Exceptions.BucketNotFoundException();
                }
            }
            catch (MinioException e)
            {
                throw;
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
