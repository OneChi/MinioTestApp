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


        //  BUCKET OPERATIONS

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
            catch (MinioException e) { throw; }
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
        public Task RemoveBucketAsync(String BucketName)
        {
            // TODO handle situation when bucket are not empty
            try
            {
                // Check if my-bucket exists before removing it.
                var found = minio.BucketExistsAsync(BucketName);
                Task.WaitAll(found);
                if (found.Result)
                {
                    var remove = minio.RemoveBucketAsync(BucketName);

                    return remove;
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
        //Get bucket policy.
        public Task<String> GetBucketPolicyAsync(string bucketName)
        {
            try
            {

                var outStr = minio.GetPolicyAsync(bucketName);
                return outStr;

            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Set bucket policy.
        public Task SetBucketPolicyAsync(string bucketName, string policyJson)
        {
            try
            {
                var outStr = minio.SetPolicyAsync(bucketName, policyJson);
                return outStr;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        // Sets notification configuration for a given bucket
        public Task SetBucketNotificAsync(string bucketName, BucketNotification notification)
        {
            try
            {
                /*
                 BucketNotification notification = new BucketNotification();
                    Arn topicArn = new Arn("aws", "sns", "us-west-1", "412334153608", "topicminio");

                    TopicConfig topicConfiguration = new TopicConfig(topicArn);
                    List<EventType> events = new List<EventType>() { EventType.ObjectCreatedPut, EventType.ObjectCreatedCopy };
                    topicConfiguration.AddEvents(events);
                topicConfiguration.AddFilterPrefix("images");
                topicConfiguration.AddFilterSuffix("jpg");
                notification.AddTopic(topicConfiguration);

                QueueConfig queueConfiguration = new QueueConfig("arn:aws:sqs:us-west-1:482314153608:testminioqueue1");
                    queueConfiguration.AddEvents(new List<EventType>() { EventType.ObjectCreatedCompleteMultipartUpload
                });
                notification.AddQueue(queueConfiguration);
                */
                var task = minio.SetBucketNotificationsAsync(bucketName, notification);
                return task;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        // Get bucket notification configuration Task<BucketNotification> GetBucketNotificationAsync(string bucketName, CancellationToken cancellationToken = default(CancellationToken))
        public Task<BucketNotification> GetBucketNotificAsync(string bucketName, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var notifications = minio.GetBucketNotificationsAsync(bucketName, cancellationToken);
                return notifications;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //RemoveAllBucketNotificationsAsync(string bucketName)
        public Task RemoveAllBucketNotificAsync(string bucketName)
        {
            try
            {
                var task = minio.RemoveAllBucketNotificationsAsync(bucketName);
                return task;
            }
            catch (MinioException e)
            {
                throw;
            }
        }

        //   TODO:   OBJECT OPERATIONS
        //Downloads an object as a stream.

        //Downloads the specified range bytes of an object as a stream.Both offset and length are required

        //Downloads and saves the object as a file in the local filesystem.

        //Uploads contents from a stream to objectName.
        //The maximum size of a single object is limited to 5TB. putObject transparently uploads objects larger than 5MiB in multiple parts. Uploaded data is carefully verified using MD5SUM signatures.

        //Uploads contents from a file to objectName.

        //Gets metadata of an object.

        //Copies content from objectName to destObjectName.

        //Removes an object.

        //Removes a list of objects.

        //Removes a list of objects.

        //  TODO:   PRESIGNED OPERATIONS 

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
