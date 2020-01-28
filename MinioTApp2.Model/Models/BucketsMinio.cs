namespace MinioTApp2.Model.Models
{
    public class BucketsMinio
    {

        public string BucketName { get; private set; }
        public string BucketDesc { get; private set; }
        public string Name => BucketName;

        public BucketsMinio(string bucketName, string bucketDesc)
        {
            BucketName = bucketName;
            BucketDesc = bucketDesc;
        }

    }
}
