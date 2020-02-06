using Minio.DataModel;
using System;
using System.Collections.Generic;

namespace MinioTApp2.Model.Models
{
    class MinioObjectStatModel
    {
        public string ObjectName { get; }
        public long Size { get; }
        public DateTime LastModified { get; }
        public string ETag { get; }
        public string ContentType { get; }
        public Dictionary<string, string> MetaData { get; }


        public MinioObjectStatModel(ObjectStat ojectStat)
        {
            ObjectName = ojectStat.ObjectName;
            Size = ojectStat.Size;
            LastModified = ojectStat.LastModified;
            ETag = ojectStat.ETag;
            ContentType = ojectStat.ContentType;
            MetaData = ojectStat.MetaData;
        }
    }
}

