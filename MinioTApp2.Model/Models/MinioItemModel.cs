using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Minio.DataModel;
using System.Threading.Tasks;

namespace MinioTApp2.Model.Models
{
    public class MinioItemModel
    { 
        public string ItemKey { get; private set; }
        public string Etag { get; private set; }
        public bool ItemIsDir { get; private set; }
        public string ItemLastModifiedDate { get; private set; }
        public DateTime? ItemLastModifiedDateTime { get; private set; }
        public ulong ItemSize { get; private set; }


        public MinioItemModel(string ItemName)
        {
            Etag = null;
            ItemLastModifiedDate = "000";
            ItemLastModifiedDateTime = new DateTime?();
            ItemIsDir = false;
            ItemKey = ItemName;
            ItemSize = 0;

        }

        public MinioItemModel(Item minioItem) 
        {
            Etag = minioItem.ETag;
            ItemLastModifiedDate = minioItem.LastModified;
            ItemLastModifiedDateTime = minioItem.LastModifiedDateTime;
            ItemIsDir = minioItem.IsDir;
            ItemKey = minioItem.Key;
            ItemSize = minioItem.Size;
        }
    }
}
