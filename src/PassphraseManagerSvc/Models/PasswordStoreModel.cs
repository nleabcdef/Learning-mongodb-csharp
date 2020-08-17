using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PassphraseManagerSvc.Models
{
    public class PasswordStoreModel : IModel
    {
        public string Name { get; set; }
        public string Key { get; set; }
        [BsonElement("passphrase")]
        public string PassPhrase { get; set; }
        [BsonElement("isencrypted")]
        public bool IsEncrypted { get; set; }
        [BsonElement("encryptedby")]
        public string EncryptedBy { get; set; }

        public StoreItemModel[] Passwords { get; set; }

        [BsonElement("createdon")]
        public string CreatedOn { get; set; }
        [BsonElement("updatedon")]
        public string UpdatedOn { get; set; }

        private string _id;

        [BsonElement("_id")]
        //[JsonProperty("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {
            get { return _id;}
            set { _id = value;}
        }
    }
    
}