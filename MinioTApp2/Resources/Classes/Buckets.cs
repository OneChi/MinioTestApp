using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioTApp2.Resources.Classes
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
