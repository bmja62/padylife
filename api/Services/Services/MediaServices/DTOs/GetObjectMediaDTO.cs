using Entities.Medias;

namespace Services.Services.MediaServices.DTOs
{
    public class GetObjectMediaDTO
    {
        public GetObjectMediaDTO(long id, string url, MediaTypes type, string typeName, long objectId)
        {
            Id = id;
            Url = url;
            Type = type;
            TypeName = typeName;
            ObjectId = objectId;
        }

        public long Id { get; }
        public string Url { get; }
        public MediaTypes Type { get; }
        public string TypeName { get; }
        public long ObjectId { get; }

        internal static GetObjectMediaDTO Create(long id, string url, MediaTypes type, string typeName, long objectId)
        => new(id, url, type, typeName, objectId);
    }
}
