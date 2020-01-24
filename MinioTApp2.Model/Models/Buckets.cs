namespace MinioTApp2.Model.Models
{
    public class Buckets
    {

        public string BucketName { get; private set; }
        public string BucketDesc { get; private set; }
        public string Name => BucketName;

        public Buckets(string bucketName, string bucketDesc)
        {
            BucketName = bucketName;
            BucketDesc = bucketDesc;
        }

    }
}
