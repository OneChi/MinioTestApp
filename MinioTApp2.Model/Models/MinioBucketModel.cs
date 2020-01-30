using System;

namespace MinioTApp2.Model.Models
{
    public class MinioBucketModel
    {

        public string BucketName { get; private set; }
        public string BucketCreationDate { get; private set; }
        public DateTime BucketCreationDateTime { get; private set; }
        public string Name => BucketName;

        public MinioBucketModel(string bucketName, string bucketDesc)
        {
            BucketName = bucketName;
            BucketCreationDate = bucketDesc;
            BucketCreationDateTime = new DateTime();
        }

    }
}
