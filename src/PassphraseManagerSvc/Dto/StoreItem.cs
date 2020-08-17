using System;
using System.ComponentModel.DataAnnotations;

namespace PassphraseManagerSvc.Dto
{
    public class StoreItem
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public string Url { get; set; }
        [Required]
        public string Description { get; set; }
        public string Email { get; set; }
        [Required]
        public string IsActive { get; set; }

        public bool SecuityPolicyEnabled { get; set; }
        public Interval PasswordPolicyCycle { get; set; }
        public int PassordPolicyValue { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string Category { get; set; }
        public string[] Tags { get; set; }
    }

}