using Minio.DataModel;

namespace MinioTApp2.Model.Models
{
    class MinioUploadModel
    {
        public MinioUploadModel(Upload upload)
        {
            Key = upload.Key;
            UploadId = upload.UploadId;
            Initiated = upload.Initiated;
        }

        public string Key { get; private set; }
        public string UploadId { get; private set; }
        public string Initiated { get; private set; }
    }
}
