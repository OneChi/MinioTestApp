using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public Task GetObjectAsStreamAsync(string bucketName, string objectName, Action<Stream> callback, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Check whether the object exists using statObject().
                // If the object is not found, statObject() throws an exception,
                // else it means that the object exists.
                // Execution is successful.
                var taskStatObject = minio.StatObjectAsync(bucketName, objectName);
                Task.WaitAll(taskStatObject);
                // Get input stream to have content of 'my-objectname' from 'my-bucketname'
                var taskGetObject = minio.GetObjectAsync(bucketName, objectName,
                                                 (stream) =>
                                                 {
                                                     stream.CopyTo(Console.OpenStandardOutput());
                                                 }, sse, cancellationToken);
                return taskGetObject;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Downloads the specified range bytes of an object as a stream.Both offset and length are required
        public Task GetObjectRangeAsync(string bucketName, string objectName, long offset, long length, Action<Stream> callback, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Check whether the object exists using statObject().
                // If the object is not found, statObject() throws an exception,
                // else it means that the object exists.
                // Execution is successful.
                var taskStatObj = minio.StatObjectAsync(bucketName, objectName);
                Task.WaitAll(taskStatObj);
                // Get input stream to have content of 'my-objectname' from 'my-bucketname'
                var taskGetObj = minio.GetObjectAsync(bucketName, objectName, offset, length,
                                                 (stream) =>
                                                 {
                                                     stream.CopyTo(Console.OpenStandardOutput());
                                                 }, sse, cancellationToken);
                return taskGetObj;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Downloads and saves the object as a file in the local filesystem.
        public Task GetObjectByFileAsync(string bucketName, string objectName, string fileName, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Check whether the object exists using statObjectAsync().
                // If the object is not found, statObjectAsync() throws an exception,
                // else it means that the object exists.
                // Execution is successful.
                var taskStatObj = minio.StatObjectAsync(bucketName, objectName);
                Task.WaitAll(taskStatObj);
                // Gets the object's data and stores it in photo.jpg
                var taskGetObj = minio.GetObjectAsync(bucketName, objectName, fileName);
                return taskGetObj;

            }
            catch (MinioException e)
            {
                throw;

            }
        }
        //Uploads contents from a stream to objectName.
        //The maximum size of a single object is limited to 5TB. putObject transparently uploads objects larger than 5MiB in multiple parts. Uploaded data is carefully verified using MD5SUM signatures.
        public Task PutObjectFromStreamAsync(string bucketName, string objectName, Stream data, long size, string contentType = "application/octet-stream", Dictionary<string, string> metaData = null, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                /*
                byte[] bs = File.ReadAllBytes(fileName);
                System.IO.MemoryStream filestream = new System.IO.MemoryStream(bs);
                // Specify SSE-C encryption options
                Aes aesEncryption = Aes.Create();
                aesEncryption.KeySize = 256;
                aesEncryption.GenerateKey();
                var ssec = new SSEC(aesEncryption.Key);*/

                var taskUploadBucket = minio.PutObjectAsync(bucketName,
                                           objectName,
                                            data,
                                            size,
                                           contentType, metaData, sse, cancellationToken);
                /*
                 

                await minio.PutObjectAsync("mybucket",
                               "island.jpg",
                                filestream,
                                filestream.Length,
                               "application/octet-stream", ssec);

                 
                 */
                return taskUploadBucket;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Uploads contents from a file to objectName.
        public Task PutObjectFromFileAsync(string bucketName, string objectName, string filePath, string contentType = null, Dictionary<string, string> metaData = null, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var taskUploadObject = minio.PutObjectAsync(bucketName, objectName, filePath, contentType, metaData, sse, cancellationToken);
                return taskUploadObject;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Gets metadata of an object.
        public Task<ObjectStat> StatOfObjectAsync(string bucketName, string objectName, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Get the metadata of the object.
                var objectStat = minio.StatObjectAsync("mybucket", "myobject");
                return objectStat;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Copies content from objectName to destObjectName.
        public Task CopyObjectFromToAsync(string bucketName, string objectName, string destBucketName, string destObjectName = null, CopyConditions copyConditions = null, Dictionary<string, string> metadata = null, ServerSideEncryption sseSrc = null, ServerSideEncryption sseDest = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                /*
                CopyConditions copyConditions = new CopyConditions();
                copyConditions.setMatchETagNone("TestETag");
                ServerSideEncryption sseSrc, sseDst;
                */
                // Uncomment to specify source and destination Server-side encryption options
                /*
                 Aes aesEncryption = Aes.Create();
                 aesEncryption.KeySize = 256;
                 aesEncryption.GenerateKey();
                 sseSrc = new SSEC(aesEncryption.Key);
                 sseDst = new SSES3();
                */
                var taskCopyObj = minio.CopyObjectAsync(bucketName, objectName, destBucketName, destObjectName, copyConditions, metadata, sseSrc, sseDest, cancellationToken);
                return taskCopyObj;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Removes an object.
        public Task RemoveObjectFromServerAsync(string bucketName, string objectName, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Remove objectname from the bucket my-bucketname.
                var taskRemoveObj = minio.RemoveObjectAsync(bucketName,objectName);
                return taskRemoveObj;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Removes a list of objects.
        public Task<IObservable<DeleteError>> RemoveObjectListFromServerAsync(string bucketName, IEnumerable<string> objectsList, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                /*List<String> objectNames = new LinkedList<String>();
                objectNames.add("my-objectname1");
                objectNames.add("my-objectname2");
                objectNames.add("my-objectname3");*/


                // Remove list of objects in objectNames from the bucket bucketName.
                var observable =  minio.RemoveObjectAsync(bucketName, objectsList,cancellationToken);

                return observable;
                /*IDisposable subscription = observable.Subscribe(
                    deleteError => Console.WriteLine("Object: {0}", deleteError.Key),
                    ex => Console.WriteLine("OnError: {0}", ex),
                    () =>
                    {
                        Console.WriteLine("Listed all delete errors for remove objects on  " + bucketName + "\n");
                    });*/
            }
            catch (MinioException e)
            {
                throw;
            }
        }
        //Removes a partially uploaded object.
        public Task RemoveIncompleteUploadAsync(string bucketName, string objectName, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Removes partially uploaded objects from buckets.
                var taskDelete = minio.RemoveIncompleteUploadAsync(bucketName, objectName, cancellationToken);
                return taskDelete;
            }
            catch (MinioException e)
            {
                throw;
            }
        }
       
        //  TODO:   PRESIGNED OPERATIONS - ? am i need this

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
