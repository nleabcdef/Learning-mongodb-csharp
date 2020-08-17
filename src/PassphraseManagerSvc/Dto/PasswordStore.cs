using System;
using System.ComponentModel.DataAnnotations;

namespace PassphraseManagerSvc.Dto
{
    public class PasswordStore
    {
        public string Id {get; set;}

        [Required]
        public string Name { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string PassPhrase { get; set; }
        [Required]
        public bool IsEncrypted { get; set; }
        public EncryptedType EncryptedBy { get; set; }

        public StoreItem[] Passwords { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

}