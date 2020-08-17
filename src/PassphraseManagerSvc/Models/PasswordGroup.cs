using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PassphraseManagerSvc.Models
{
    public class PasswordsCategory
    {
        [BsonElement("_id")]
        public string Category {get; set;}
        public StoreItemModel[] Passwords {get; set;}

        public int Count { get { return Passwords.Length;} }
    }

    public class PasswordsByCategory :IModel
    {
        public string Name { get; set; }
        public string Key { get; set; }
        
        private string _id;

        [BsonElement("_id")]
        //[JsonProperty("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {
            get { return _id;}
            set { _id = value;}
        }

        public PasswordsCategory[] Passwords {get; set;}
    }
}