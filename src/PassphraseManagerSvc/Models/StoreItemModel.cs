using MongoDB.Bson.Serialization.Attributes;

namespace PassphraseManagerSvc.Models
{
    public class StoreItemModel 
    {
        public string Title { get; set; }
        [BsonElement("username")]
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Url { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        [BsonElement("isactive")]
        public string IsActive { get; set; }

        [BsonElement("secuitypolicyenabled")]
        public bool SecuityPolicyEnabled { get; set; }
        [BsonElement("passwordpolicycycle")]
        public string PasswordPolicyCycle { get; set; }
        [BsonElement("passordpolicyvalue")]
        public int PassordPolicyValue { get; set; }

         [BsonElement("createdon")]
        public string CreatedOn { get; set; }
        [BsonElement("updatedon")]
        public string UpdatedOn { get; set; }

        public string Category { get; set; }
        public string[] Tags { get; set; }
    }
}