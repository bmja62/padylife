using Entities.Common;

namespace Entities.Medias
{
    public class Media : BaseEntity<long>
    {
        public Media(string uRL, long objectId, MediaTypes type)
        {
            URL = uRL;
            ObjectId = objectId;
            Type = type;
        }

        public string URL { get; private set; }
        public long ObjectId { get; private set; }
        public MediaTypes Type { get; private set; }


        internal void SetURL(string url) => URL = url;

        internal void SetObjectId(long id) => ObjectId = id;

        internal void SetType(MediaTypes type) => Type = type;
    }
}
